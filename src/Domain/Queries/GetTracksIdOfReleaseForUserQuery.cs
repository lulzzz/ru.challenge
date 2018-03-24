using MediatR;
using System;
using System.Collections.Generic;

namespace RU.Challenge.Domain.Queries
{
    public class GetTracksIdOfReleaseForUserQuery : IRequest<IEnumerable<Guid>>
    {
        public Guid ReleaseId { get; set; }

        public Guid UserId { get; set; }

        public GetTracksIdOfReleaseForUserQuery(Guid releaseId, Guid userId)
        {
            ReleaseId = releaseId;
            UserId = userId;
        }
    }
}