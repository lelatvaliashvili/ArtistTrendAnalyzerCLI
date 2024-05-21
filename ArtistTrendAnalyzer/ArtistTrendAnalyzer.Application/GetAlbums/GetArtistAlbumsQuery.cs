using ArtistTrendAnalyzer.Domain.Entities;
using MediatR;

namespace ArtistTrendAnalyzer.Application.GetAlbums;

public class GetArtistAlbumsQuery(string artistName) : IRequest<List<Album>>
{
    public string ArtistName { get; set; } = artistName;
}
