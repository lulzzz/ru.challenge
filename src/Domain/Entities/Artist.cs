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

        private Artist()
        {
        }

        public static Artist Create(int age, string name)
            => new Artist(Guid.NewGuid(), age, name);
    }
}