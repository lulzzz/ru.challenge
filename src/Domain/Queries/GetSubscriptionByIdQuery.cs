using MediatR;
using System;

namespace RU.Challenge.Domain.Queries
{
    public class GetSubscriptionByIdQuery : IRequest<Entities.Subscription>
    {
        public Guid Id { get; set; }

        public GetSubscriptionByIdQuery(Guid id)
            => Id = id;
    }
}