using RU.Challenge.Domain.Commands;
using System;

namespace RU.Challenge.Domain.Events
{
    public class CreateArtistEvent
    {
        public Guid Id { get; private set; }

        public int Age { get; private set; }

        public string Name { get; private set; }

        public CreateArtistEvent(Guid id, int age, string name)
        {
            Id = id;
            Age = age;
            Name = name;
        }

        public static CreateArtistEvent CreateFromCommand(CreateArtistCommand command, Guid id)
            => new CreateArtistEvent(id, command.Age, command.Name);
    }
}