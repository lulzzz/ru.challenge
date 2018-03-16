using System;

namespace RU.Challenge.Domain.Entities
{
    public class Artist
    {
        public Guid Id { get; private set; }

        public int Age { get; private set; }

        public string Name { get; private set; }

        private Artist(Guid id, int age, string name) : this()
        {
            Id = id;
            Age = age;
            Name = name;
        }

        private Artist(Artist other) : this()
        {
            Id = other.Id;
            Age = other.Age;
            Name = other.Name;
        }

        private Artist()
        {
        }

        public Artist Clone() => new Artist(this);

        public static Artist Create(Guid id, int age, string name)
            => new Artist(id, age, name);
    }
}