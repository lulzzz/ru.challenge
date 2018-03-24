using MediatR;
using System;

namespace RU.Challenge.Domain.Queries
{
    public class ExistsSubscriptionByIdQuery : IRequest<bool>
    {
        public Guid Id { get; set; }

        public ExistsSubscriptionByIdQuery(Guid id)
            => Id = id;
    }
}