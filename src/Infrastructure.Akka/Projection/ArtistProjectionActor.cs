using Akka.Actor;
using Infrastructure.Repositories;
using RU.Challenge.Domain.Events;
using System;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Akka.Projection
{
    public class ArtistProjectionActor : ReceiveActor
    {
        public ArtistProjectionActor(IArtistRepository artistRepository)
        {
            Receive<CreateArtistEvent>(e => artistRepository.AddAsync(e));
        }

        protected override void PreStart()
        {
            Context.System.EventStream.Subscribe(Self, typeof(CreateArtistEvent));
        }
    }
}