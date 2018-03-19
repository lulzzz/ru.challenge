using MediatR;
using System;

namespace RU.Challenge.Domain.Commands
{
    public class AddDistributionPlatformToSubscriptionCommand : IRequest
    {
        public Guid SubscriptionId { get; set; }

        public Guid DistributionPlatformId { get; set; }

        public AddDistributionPlatformToSubscriptionCommand(
            Guid subscriptionId, Guid distributionPlatformId)
        {
            SubscriptionId = subscriptionId;
            DistributionPlatformId = distributionPlatformId;
        }
    }
}