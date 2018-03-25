using Akka.Actor;
using Infrastructure.Repositories;
using RU.Challenge.Domain.Events;

namespace RU.Challenge.Infrastructure.Akka.Projection
{
    public class GenreProjectionActor : ReceiveActor
    {
        public GenreProjectionActor(IGenreRepository genreRepository)
        {
            if (genreRepository == null)
                throw new System.ArgumentNullException(nameof(genreRepository));

            Receive<CreateGenreEvent>(e => genreRepository.AddAsync(e));
        }

        protected override void PreStart()
        {
            Context.System.EventStream.Subscribe(Self, typeof(CreateGenreEvent));
        }
    }
}