using MediatR;
using System;

namespace RU.Challenge.Domain.Commands
{
    public class CreateDistributionPlatformCommand : IRequest
    {
        public Guid DistributionPlatformId { get; private set; }

        public string Name { get; set; }

        public CreateDistributionPlatformCommand(string name)
            => Name = name;

        public void SetId(Guid distributionPlatformId)
        {
            DistributionPlatformId = distributionPlatformId;
        }
    }
}