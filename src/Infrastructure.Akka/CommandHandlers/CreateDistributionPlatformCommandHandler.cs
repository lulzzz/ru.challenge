using Akka.Actor;
using MediatR;
using RU.Challenge.Domain.Commands;
using RU.Challenge.Infrastructure.Akka.Actors;
using System.Threading;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Akka.CommandHandlers
{
    public class CreateDistributionPlatformCommandHandler : IRequestHandler<CreateDistributionPlatformCommand>
    {
        private readonly ActorSystem _actorSystem;

        public CreateDistributionPlatformCommandHandler(ActorSystem actorSystem)
            => _actorSystem = actorSystem;

        public Task Handle(CreateDistributionPlatformCommand message, CancellationToken cancellationToken)
        {
            var distributionPlatformActor = _actorSystem.ActorOf(DistributionPlatformActor.GetProps(message.DistributionPlatformId));
            distributionPlatformActor.Tell(message);
            return Task.CompletedTask;
        }
    }
}