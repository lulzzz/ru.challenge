using MediatR;
using System;
using System.Collections.Generic;

namespace RU.Challenge.Domain.Queries
{
    public class GetArtistsByIdQuery : IRequest<IEnumerable<Entities.Artist>>
    {
        public IEnumerable<Guid> Ids { get; set; }

        public GetArtistsByIdQuery(IEnumerable<Guid> ids)
            => Ids = ids;
    }
}