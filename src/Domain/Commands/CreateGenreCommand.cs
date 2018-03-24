using MediatR;
using System;

namespace RU.Challenge.Domain.Commands
{
    public class CreateGenreCommand : IRequest
    {
        public Guid GenreId { get; private set; }

        public string Name { get; set; }

        public CreateGenreCommand(string name)
            => Name = name;

        public void SetId(Guid genreId)
        {
            GenreId = genreId;
        }
    }
}