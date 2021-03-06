﻿using Akka.Actor;
using MediatR;
using RU.Challenge.Domain.Commands;
using RU.Challenge.Infrastructure.Akka.Actors;
using System.Threading;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Akka.CommandHandlers
{
    public class CreateReleaseCommandHandler : IRequestHandler<CreateReleaseCommand>
    {
        private readonly ActorSystem _actorSystem;

        public CreateReleaseCommandHandler(ActorSystem actorSystem)
            => _actorSystem = actorSystem ?? throw new System.ArgumentNullException(nameof(actorSystem));

        public Task Handle(CreateReleaseCommand message, CancellationToken cancellationToken)
        {
            var releaseActor = _actorSystem.ActorOf(ReleaseActor.GetProps(message.ReleaseId));
            releaseActor.Tell(message);
            return Task.CompletedTask;
        }
    }
}