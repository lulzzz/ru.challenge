using Akka.Actor;
using MediatR;
using RU.Challenge.Domain.Commands;
using RU.Challenge.Infrastructure.Akka.Actors;
using System.Threading;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Akka.CommandHandlers
{
    public class CreateGenreCommandHandler : IRequestHandler<CreateGenreCommand>
    {
        private readonly ActorSystem _actorSystem;

        public CreateGenreCommandHandler(ActorSystem actorSystem)
            => _actorSystem = actorSystem ?? throw new System.ArgumentNullException(nameof(actorSystem));

        public Task Handle(CreateGenreCommand message, CancellationToken cancellationToken)
        {
            var genreActor = _actorSystem.ActorOf(GenreActor.GetProps(message.GenreId));
            genreActor.Tell(message);
            return Task.CompletedTask;
        }
    }
}