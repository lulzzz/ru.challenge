using MediatR;
using System.Collections.Generic;

namespace RU.Challenge.Domain.Queries
{
    public class GetAllSubscriptionsQuery : IRequest<IEnumerable<Entities.Subscription>>
    {
    }
}