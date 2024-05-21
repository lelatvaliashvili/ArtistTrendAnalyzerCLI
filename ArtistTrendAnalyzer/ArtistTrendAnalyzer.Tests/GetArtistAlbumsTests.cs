using ArtistTrendAnalyzer.Application.GetAlbums;
using ArtistTrendAnalyzer.Domain.Entities;
using ArtistTrendAnalyzer.Domain.Interfaces;
using Moq;

namespace ArtistTrendAnalyzer.Tests;

public class GetArtistAlbumsTests
{
    [Fact]
    public async Task GetArtistAlbumsHandler_ValidRequest_ReturnsAlbums()
    {
        // Arrange
        var artistServiceMock = new Mock<IArtistService>();
        GenerateArtistData(artistServiceMock);


        // Act
        var handler = new GetArtistAlbumsQueryHandler(artistServiceMock.Object);
        var query = new GetArtistAlbumsQuery("ArtistName");
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task AnalyzeAlbumsData_CorrectAnalysis()
    {
        // Arrange
        var artistServiceMock = new Mock<IArtistService>();
        var albums = GenerateArtistData(artistServiceMock);
        var averageTracksPerAlbum = albums.Average(album => album.TotalTracks);
        var releaseDates = albums.GroupBy(album => album.ReleaseYear)
                                .Select(g => new { Year = g.Key, Count = g.Count() })
                                .OrderBy(x => x.Year);

        // Act
        var handler = new GetArtistAlbumsQueryHandler(artistServiceMock.Object);
        var query = new GetArtistAlbumsQuery("ArtistName");
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(9, averageTracksPerAlbum);
        Assert.Collection(releaseDates,
            item => Assert.Equal(2020, item.Year),
            item => Assert.Equal(2022, item.Year)
        );
    }

    private static List<Album> GenerateArtistData(Mock<IArtistService> artistServiceMock)
    {
        var albums = new List<Album>
        {
            new Album { Name = "Album1", ReleaseYear = 2022, TotalTracks = 10, Genres = new List<string> { "Genre1", "Genre2" } },
            new Album { Name = "Album2", ReleaseYear = 2020, TotalTracks = 8, Genres = new List<string> { "Genre3", "Genre4" } }
        };
        artistServiceMock.Setup(service => service.GetArtistAlbumsAsync(It.IsAny<string>()))
            .ReturnsAsync(albums);

        return albums;
    }
}