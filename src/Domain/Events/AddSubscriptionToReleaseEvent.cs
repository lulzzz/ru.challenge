using RU.Challenge.Domain.Commands;
using System;

namespace RU.Challenge.Domain.Events
{
    public class AddSubscriptionToReleaseEvent
    {
        public Guid ReleaseId { get; set; }

        public Guid SubscriptionId { get; set; }

        public AddSubscriptionToReleaseEvent(Guid releaseId, Guid subscriptionId)
        {
            ReleaseId = releaseId;
            SubscriptionId = subscriptionId;
        }

        public static AddSubscriptionToReleaseEvent CreateFromCommand(AddSubscriptionToReleaseCommand command)
            => new AddSubscriptionToReleaseEvent(command.ReleaseId, command.SubscriptionId);
    }
}