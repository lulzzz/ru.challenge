using System;
using System.Collections.Generic;
using System.Text;

namespace RU.Challenge.Domain.Commands
{
    public class CreateDistributionPlatformCommand
    {
        public string Name { get; private set; }

        public CreateDistributionPlatformCommand(string name)
        {
            Name = name;
        }
    }
}
