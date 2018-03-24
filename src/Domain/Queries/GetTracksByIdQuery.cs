using MediatR;
using System;
using System.Collections.Generic;

namespace RU.Challenge.Domain.Queries
{
    public class GetTracksByIdQuery : IRequest<IEnumerable<Entities.Track>>
    {
        public IEnumerable<Guid> Ids { get; set; }

        public GetTracksByIdQuery(IEnumerable<Guid> ids)
            => Ids = ids;
    }
}