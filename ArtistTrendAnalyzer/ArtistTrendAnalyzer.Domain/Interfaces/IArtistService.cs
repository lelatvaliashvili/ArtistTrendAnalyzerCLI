using ArtistTrendAnalyzer.Domain.Entities;

namespace ArtistTrendAnalyzer.Domain.Interfaces;

public interface IArtistService
{
    Task<List<Album>> GetArtistAlbumsAsync(string artistName);
    Task<string> GetArtistIdAsync(string artistName);
}
