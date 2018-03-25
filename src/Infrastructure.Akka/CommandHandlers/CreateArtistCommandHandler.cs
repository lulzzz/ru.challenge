using Akka.Actor;
using MediatR;
using RU.Challenge.Domain.Commands;
using RU.Challenge.Infrastructure.Akka.Actors;
using System.Threading;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Akka.CommandHandlers
{
    public class CreateArtistCommandHandler : IRequestHandler<CreateArtistCommand>
    {
        private readonly ActorSystem _actorSystem;

        public CreateArtistCommandHandler(ActorSystem actorSystem)
            => _actorSystem = actorSystem ?? throw new System.ArgumentNullException(nameof(actorSystem));

        public Task Handle(CreateArtistCommand message, CancellationToken cancellationToken)
        {
            var artistActor = _actorSystem.ActorOf(ArtistActor.GetProps(message.ArtistId));
            artistActor.Tell(message);
            return Task.CompletedTask;
        }
    }
}