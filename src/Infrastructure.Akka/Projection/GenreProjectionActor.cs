using Akka.Actor;
using Infrastructure.Repositories;
using RU.Challenge.Domain.Events;
using System;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Akka.Projection
{
    public class GenreProjectionActor : ReceiveActor
    {
        public GenreProjectionActor(IGenreRepository genreRepository)
        {
            Receive<CreateGenreEvent>(e => genreRepository.AddAsync(e));
        }

        protected override void PreStart()
        {
            Context.System.EventStream.Subscribe(Self, typeof(CreateGenreEvent));
        }
    }
}