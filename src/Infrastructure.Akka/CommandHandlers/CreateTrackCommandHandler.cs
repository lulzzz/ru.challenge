using Akka.Actor;
using MediatR;
using RU.Challenge.Domain.Commands;
using RU.Challenge.Infrastructure.Akka.Actors;
using System.Threading;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Akka.CommandHandlers
{
    public class CreateTrackCommandHandler : IRequestHandler<CreateTrackCommand>
    {
        private readonly ActorSystem _actorSystem;

        public CreateTrackCommandHandler(ActorSystem actorSystem)
            => _actorSystem = actorSystem;

        public Task Handle(CreateTrackCommand message, CancellationToken cancellationToken)
        {
            var releaseActor = _actorSystem.ActorOf(ReleaseActor.GetProps(message.ReleaseId));
            releaseActor.Tell(message);
            return Task.CompletedTask;
        }
    }
}