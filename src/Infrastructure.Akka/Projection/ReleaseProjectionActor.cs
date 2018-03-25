using Akka.Actor;
using Infrastructure.Repositories;
using RU.Challenge.Domain.Events;

namespace RU.Challenge.Infrastructure.Akka.Projection
{
    public class ReleaseProjectionActor : ReceiveActor
    {
        public ReleaseProjectionActor(IReleaseRepository releaseRepository)
        {
            if (releaseRepository == null)
                throw new System.ArgumentNullException(nameof(releaseRepository));

            Receive<CreateReleaseEvent>(e => releaseRepository.AddAsync(e));
            Receive<AddSubscriptionToReleaseEvent>(e => releaseRepository.AddSubscriptionAsync(e));
        }

        protected override void PreStart()
        {
            Context.System.EventStream.Subscribe(Self, typeof(CreateReleaseEvent));
            Context.System.EventStream.Subscribe(Self, typeof(AddSubscriptionToReleaseEvent));
        }
    }
}