using ArtistTrendAnalyzer.Application.GetAlbums;
using ArtistTrendAnalyzer.Domain.Entities;
using ArtistTrendAnalyzer.Domain.Interfaces;
using ArtistTrendAnalyzer.Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// Build configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory()) 
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// Setup DI
var serviceProvider = new ServiceCollection()
    .AddSingleton<IArtistService, SpotifyArtistService>()
    .AddSingleton<IConfiguration>(configuration)
    .AddMediatR(config =>
    {
        config.RegisterServicesFromAssembly(typeof(GetArtistAlbumsQuery).Assembly);
    })
    .BuildServiceProvider();

var mediator = serviceProvider.GetRequiredService<IMediator>();

Console.Write("Enter artist name: ");
string? artistName = Console.ReadLine();

if (string.IsNullOrEmpty(artistName))
{
    Console.WriteLine("Name Cannot be Empty");
    return;
}

var query = new GetArtistAlbumsQuery(artistName);
List<Album> albums = await mediator.Send(query);

if (albums.Count == 0)
{
    Console.WriteLine("No albums found for this artist.");
    return;
}

AnalyzeAndDisplayAlbums(albums);
    

static void AnalyzeAndDisplayAlbums(List<Album> albums)
{
    // Calculate the average popularity
    double averagePopularity = albums.Average(album => album.TotalTracks);
    Console.WriteLine($"Average number of tracks per album: {averagePopularity}");

    // Display a trend of album releases over time
    var releaseDates = albums.GroupBy(album => album.ReleaseYear)
                             .Select(g => new { Year = g.Key, Count = g.Count() })
                             .OrderBy(x => x.Year);

    Console.WriteLine("Album release trend over the years:");
    foreach (var year in releaseDates)
    {
        Console.WriteLine($"{year.Year}: {year.Count} albums");
    }

    // Perform genre analysis
    var genreCounts = albums.SelectMany(album => album.Genres)
                            .GroupBy(genre => genre)
                            .Select(g => new { Genre = g.Key, Count = g.Count() })
                            .OrderByDescending(x => x.Count);

    Console.WriteLine("Genre analysis:");
    foreach (var genre in genreCounts)
    {
        Console.WriteLine($"{genre.Genre}: {genre.Count} occurrences");
    }
}