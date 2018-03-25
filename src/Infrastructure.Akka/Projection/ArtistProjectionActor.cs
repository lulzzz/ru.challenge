using Akka.Actor;
using Infrastructure.Repositories;
using RU.Challenge.Domain.Events;

namespace RU.Challenge.Infrastructure.Akka.Projection
{
    public class ArtistProjectionActor : ReceiveActor
    {
        public ArtistProjectionActor(IArtistRepository artistRepository)
        {
            if (artistRepository == null)
                throw new System.ArgumentNullException(nameof(artistRepository));

            Receive<CreateArtistEvent>(e => artistRepository.AddAsync(e));
        }

        protected override void PreStart()
        {
            Context.System.EventStream.Subscribe(Self, typeof(CreateArtistEvent));
        }
    }
}