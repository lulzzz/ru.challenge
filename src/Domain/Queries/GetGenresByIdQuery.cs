using MediatR;
using System;
using System.Collections.Generic;

namespace RU.Challenge.Domain.Queries
{
    public class GetGenresByIdQuery : IRequest<IEnumerable<Entities.Genre>>
    {
        public IEnumerable<Guid> Ids { get; set; }

        public GetGenresByIdQuery(IEnumerable<Guid> ids)
            => Ids = ids;
    }
}