using RU.Challenge.Domain.Commands;
using System;

namespace RU.Challenge.Domain.Events
{
    public class CreateGenreEvent
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public CreateGenreEvent(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public static CreateGenreEvent CreateFromCommand(CreateGenreCommand command)
            => new CreateGenreEvent(command.GenreId, command.Name);
    }
}