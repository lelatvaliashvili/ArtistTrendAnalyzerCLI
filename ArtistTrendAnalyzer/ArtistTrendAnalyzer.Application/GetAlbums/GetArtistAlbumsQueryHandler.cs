using ArtistTrendAnalyzer.Domain.Entities;
using ArtistTrendAnalyzer.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtistTrendAnalyzer.Application.GetAlbums
{
    public class GetArtistAlbumsQueryHandler : IRequestHandler<GetArtistAlbumsQuery, List<Album>>
    {
        private readonly IArtistService _artistService;

        public GetArtistAlbumsQueryHandler(IArtistService artistService)
        {
            _artistService = artistService;
        }

        public async Task<List<Album>> Handle(GetArtistAlbumsQuery request, CancellationToken cancellationToken)
        {
            return await _artistService.GetArtistAlbumsAsync(request.ArtistName);
        }
    }
}
