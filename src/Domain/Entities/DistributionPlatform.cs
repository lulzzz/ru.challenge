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

        private DistributionPlatform(DistributionPlatform other) : this()
        {
            Id = other.Id;
            Name = other.Name;
        }

        private DistributionPlatform()
        {
        }

        public DistributionPlatform Clone() => new DistributionPlatform(this);

        public static DistributionPlatform Create(Guid id, string name)
            => new DistributionPlatform(id, name);
    }
}