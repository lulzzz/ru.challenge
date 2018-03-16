using RU.Challenge.Domain.Commands;
using System;

namespace RU.Challenge.Infrastructure.Akka.Events
{
    public class CreateDistributionPlatformEvent
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public CreateDistributionPlatformEvent(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public static CreateDistributionPlatformEvent CreateFromCommand(CreateDistributionPlatformCommand command, Guid id)
            => new CreateDistributionPlatformEvent(id, command.Name);
    }
}