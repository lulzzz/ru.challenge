using MediatR;
using System;
using System.Collections.Generic;

namespace RU.Challenge.Domain.Queries
{
    public class GetReleasesByIdForUserQuery : IRequest<IEnumerable<Entities.Release>>
    {
        public IEnumerable<Guid> Ids { get; set; }

        public Guid UserId { get; set; }

        public GetReleasesByIdForUserQuery(IEnumerable<Guid> ids, Guid userId)
        {
            Ids = ids;
            UserId = userId;
        }
    }
}