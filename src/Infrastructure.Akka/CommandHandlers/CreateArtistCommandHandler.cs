using Akka.Actor;
using MediatR;
using RU.Challenge.Domain.Commands;
using RU.Challenge.Infrastructure.Akka.Actors;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Akka.CommandHandlers
{
    public class CreateArtistCommandHandler : IRequestHandler<CreateArtistCommand>
    {
        private readonly ActorSystem _actorSystem;

        public CreateArtistCommandHandler(ActorSystem actorSystem)
            => _actorSystem = actorSystem;

        public Task Handle(CreateArtistCommand message, CancellationToken cancellationToken)
        {
            var artistId = Guid.NewGuid();
            var artistActor = _actorSystem.ActorOf(ArtistActor.GetProps(artistId));
            artistActor.Tell(message);
            return Task.CompletedTask;
        }
    }
}