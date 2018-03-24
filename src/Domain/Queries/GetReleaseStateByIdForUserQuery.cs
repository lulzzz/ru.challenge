using MediatR;
using RU.Challenge.Domain.Enums;
using System;

namespace RU.Challenge.Domain.Queries
{
    public class GetReleaseStateByIdForUserQuery : IRequest<ReleaseStatus?>
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public GetReleaseStateByIdForUserQuery(Guid id, Guid userId)
        {
            Id = id;
            UserId = userId;
        }
    }
}