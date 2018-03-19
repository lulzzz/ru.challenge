using RU.Challenge.Domain.Commands;
using System;

namespace RU.Challenge.Domain.Events
{
    public class AddDistributionPlatformToSubscriptionEvent
    {
        public Guid SubscriptionId { get; private set; }

        public Guid DistributionPlatformId { get; private set; }

        public AddDistributionPlatformToSubscriptionEvent(
            Guid subscriptionId, Guid distributionPlatformId)
        {
            SubscriptionId = subscriptionId;
            DistributionPlatformId = distributionPlatformId;
        }

        public static AddDistributionPlatformToSubscriptionEvent CreateFromCommand(AddDistributionPlatformToSubscriptionCommand command)
            => new AddDistributionPlatformToSubscriptionEvent(command.SubscriptionId, command.DistributionPlatformId);
    }
}