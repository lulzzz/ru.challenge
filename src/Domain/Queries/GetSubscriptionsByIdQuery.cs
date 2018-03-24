using MediatR;
using System;
using System.Collections.Generic;

namespace RU.Challenge.Domain.Queries
{
    public class GetSubscriptionsByIdQuery : IRequest<IEnumerable<Entities.Subscription>>
    {
        public IEnumerable<Guid> Ids { get; set; }

        public GetSubscriptionsByIdQuery(IEnumerable<Guid> ids)
            => Ids = ids;
    }
}