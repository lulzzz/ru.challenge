using Akka.Actor;
using Infrastructure.Repositories;
using RU.Challenge.Domain.Events;

namespace RU.Challenge.Infrastructure.Akka.Projection
{
    public class ReleaseProjectionActor : ReceiveActor
    {
        public ReleaseProjectionActor(IReleaseRepository releaseRepository)
        {
            Receive<CreateReleaseEvent>(e => releaseRepository.AddAsync(e));
        }

        protected override void PreStart()
        {
            Context.System.EventStream.Subscribe(Self, typeof(CreateReleaseEvent));
        }
    }
}