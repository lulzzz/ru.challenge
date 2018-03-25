using Akka.Actor;
using Infrastructure.Repositories;
using RU.Challenge.Domain.Events;

namespace RU.Challenge.Infrastructure.Akka.Projection
{
    public class DistributionPlatformProjectionActor : ReceiveActor
    {
        public DistributionPlatformProjectionActor(IDistributionPlatformRepository distributionPlatformRepository)
        {
            if (distributionPlatformRepository == null)
                throw new System.ArgumentNullException(nameof(distributionPlatformRepository));

            Receive<CreateDistributionPlatformEvent>(e => distributionPlatformRepository.AddAsync(e));
        }

        protected override void PreStart()
        {
            Context.System.EventStream.Subscribe(Self, typeof(CreateDistributionPlatformEvent));
        }
    }
}