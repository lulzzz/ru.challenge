using Akka.Actor;
using Infrastructure.Repositories;
using RU.Challenge.Domain.Events;

namespace RU.Challenge.Infrastructure.Akka.Projection
{
    public class SubscriptionProjectionActor : ReceiveActor
    {
        public SubscriptionProjectionActor(ISubscriptionRepository subscriptionRepository)
        {
            if (subscriptionRepository == null)
                throw new System.ArgumentNullException(nameof(subscriptionRepository));

            Receive<CreateSubscriptionEvent>(e => subscriptionRepository.AddAsync(e));
            Receive<AddDistributionPlatformToSubscriptionEvent>(e => subscriptionRepository.AddDistributionPlatformAsync(e));
        }

        protected override void PreStart()
        {
            Context.System.EventStream.Subscribe(Self, typeof(CreateSubscriptionEvent));
            Context.System.EventStream.Subscribe(Self, typeof(AddDistributionPlatformToSubscriptionEvent));
        }
    }
}