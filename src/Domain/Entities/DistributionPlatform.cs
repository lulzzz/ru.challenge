using System;

namespace RU.Challenge.Domain.Entities
{
    public class DistributionPlatform
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; }

        private DistributionPlatform(Guid id, string name) : this()
        {
            Id = id;
            Name = name;
        }

        private DistributionPlatform()
        {
        }

        public static DistributionPlatform Create(string name)
            => new DistributionPlatform(Guid.NewGuid(), name);
    }
}