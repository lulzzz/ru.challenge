using Akka.Actor;
using Infrastructure.Repositories;
using RU.Challenge.Domain.Events;

namespace RU.Challenge.Infrastructure.Akka.Projection
{
    public class TrackProjectionActor : ReceiveActor
    {
        public TrackProjectionActor(ITrackRepository trackRepository)
        {
            if (trackRepository == null)
                throw new System.ArgumentNullException(nameof(trackRepository));

            Receive<CreateTrackEvent>(e => trackRepository.AddAsync(e));
        }

        protected override void PreStart()
        {
            Context.System.EventStream.Subscribe(Self, typeof(CreateTrackEvent));
        }
    }
}