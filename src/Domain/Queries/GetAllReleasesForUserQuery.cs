using MediatR;
using System;
using System.Collections.Generic;

namespace RU.Challenge.Domain.Queries
{
    public class GetAllReleasesForUserQuery : IRequest<IEnumerable<Entities.Release>>
    {
        public Guid UserId { get; set; }

        public GetAllReleasesForUserQuery(Guid userId)
            => UserId = userId;
    }
}