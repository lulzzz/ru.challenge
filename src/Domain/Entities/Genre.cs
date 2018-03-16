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

        private Genre(Genre other) : this()
        {
            Id = other.Id;
            Name = other.Name;
        }

        private Genre()
        {
        }

        public Genre Clone() => new Genre(this);

        public static Genre Create(Guid id, string name)
            => new Genre(id, name);
    }
}