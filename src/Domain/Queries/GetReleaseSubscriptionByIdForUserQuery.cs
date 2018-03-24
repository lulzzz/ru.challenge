using MediatR;
using RU.Challenge.Domain.Entities;
using System;

namespace RU.Challenge.Domain.Queries
{
    public class GetReleaseSubscriptionByIdForUserQuery : IRequest<Subscription>
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public GetReleaseSubscriptionByIdForUserQuery(Guid id, Guid userId)
        {
            Id = id;
            UserId = userId;
        }
    }
}