using ArtistTrendAnalyzer.Domain.Entities;
using ArtistTrendAnalyzer.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using SpotifyAPI.Web;
using System.Globalization;

namespace ArtistTrendAnalyzer.Infrastructure.Services;

public class SpotifyArtistService : IArtistService
{
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly SpotifyClient _spotify;

    public SpotifyArtistService(IConfiguration configuration)
    {
        _clientId = configuration["ClientCredentials:ClientId"]
                     ?? throw new ArgumentNullException(nameof(configuration), "ClientId cannot be null");
        _clientSecret = configuration["ClientCredentials:ClientSecret"]
            ?? throw new ArgumentNullException(nameof(configuration), "ClientSecret cannot be null");

        var config = SpotifyClientConfig.CreateDefault()
            .WithAuthenticator(new ClientCredentialsAuthenticator(_clientId, _clientSecret));
        _spotify = new SpotifyClient(config);
    }

    public async Task<List<Album>> GetArtistAlbumsAsync(string artistName)
    {
        var albumsRequest = new ArtistsAlbumsRequest { Limit = 50 };
        albumsRequest.IncludeGroupsParam = ArtistsAlbumsRequest.IncludeGroups.Album;

        var artistId = await GetArtistIdAsync(artistName);

        var albumsResponse = await _spotify.Artists.GetAlbums(artistId, albumsRequest);

        var albums = albumsResponse.Items != null ? albumsResponse.Items.Select(album => new Album
        {
            Name = album.Name,
            ReleaseYear = ExtractReleaseYear(album.ReleaseDate),
            TotalTracks = album.TotalTracks,
            Genres = new List<string>()
        }).ToList() : null;

        if (albums == null)
            throw new InvalidOperationException("There are no albums available for the specified artist");

        await FetchAlbumGenres(artistId, albums);

        return albums;
    }

    private async Task FetchAlbumGenres(string artistId, List<Album> albums)
    {
        var artist = await _spotify.Artists.Get(artistId);
        foreach (var album in albums)
        {
            album.Genres = artist.Genres;
        }
    }

    public async Task<string> GetArtistIdAsync(string artistName)
    {
        var searchRequest = new SearchRequest(SearchRequest.Types.Artist, artistName);
        var searchResponse = await _spotify.Search.Item(searchRequest);

        var artist = searchResponse.Artists.Items!.FirstOrDefault();
        return artist!.Id;
    }

    private int ExtractReleaseYear(string releaseDate)
    {
        string[] formats = { "yyyy-MM-dd", "yyyy" };
        DateTime parsedDate;

        if (DateTime.TryParseExact(releaseDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
        {
            return parsedDate.Year;
        }
        else
        {
            throw new ArgumentException("Invalid date format");
        }
    }
}
