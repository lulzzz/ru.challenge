using MediatR;
using System;

namespace RU.Challenge.Domain.Commands
{
    public class CreateGenreCommand : IRequest
    {
        public Guid GenreId { get; set; }

        public string Name { get; set; }

        public CreateGenreCommand(string name)
            => Name = name;

        public CreateGenreCommand(Guid genreId, string name)
            : this(name)
        {
            GenreId = genreId;
        }
    }
}