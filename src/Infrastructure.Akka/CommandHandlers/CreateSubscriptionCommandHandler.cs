﻿using Akka.Actor;
using MediatR;
using RU.Challenge.Domain.Commands;
using RU.Challenge.Infrastructure.Akka.Actors;
using System.Threading;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Akka.CommandHandlers
{
    public class CreateSubscriptionCommandHandler : IRequestHandler<CreateSubscriptionCommand>
    {
        private readonly ActorSystem _actorSystem;

        public CreateSubscriptionCommandHandler(ActorSystem actorSystem)
            => _actorSystem = actorSystem ?? throw new System.ArgumentNullException(nameof(actorSystem));

        public Task Handle(CreateSubscriptionCommand message, CancellationToken cancellationToken)
        {
            var subscriptionActor = _actorSystem.ActorOf(SubscriptionActor.GetProps(message.SubscriptionId));
            subscriptionActor.Tell(message);
            return Task.CompletedTask;
        }
    }
}