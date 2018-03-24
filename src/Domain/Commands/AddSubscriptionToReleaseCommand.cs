using MediatR;
using System;

namespace RU.Challenge.Domain.Commands
{
    public class AddSubscriptionToReleaseCommand : IRequest
    {
        public Guid ReleaseId { get; set; }

        public Guid SubscriptionId { get; set; }

        public AddSubscriptionToReleaseCommand(Guid releaseId, Guid subscriptionId)
        {
            ReleaseId = releaseId;
            SubscriptionId = subscriptionId;
        }
    }
}