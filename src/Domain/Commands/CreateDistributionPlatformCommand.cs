using MediatR;
using System;

namespace RU.Challenge.Domain.Commands
{
    public class CreateDistributionPlatformCommand : IRequest
    {
        public Guid DistributionPlatformId { get; set; }

        public string Name { get; set; }

        public CreateDistributionPlatformCommand(string name)
            => Name = name;

        public CreateDistributionPlatformCommand(Guid distributionPlatformId, string name)
            :this(name)
        {
            DistributionPlatformId = distributionPlatformId; 
        }
    }
}