using System;

namespace RU.Challenge.Domain.Entities
{
    public class Genre
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; }

        private Genre(Guid id, string name) : this()
        {
            Id = id;
            Name = name;
        }

        private Genre()
        {
        }

        public static Genre Create(string name)
            => new Genre(Guid.NewGuid(), name);
    }
}