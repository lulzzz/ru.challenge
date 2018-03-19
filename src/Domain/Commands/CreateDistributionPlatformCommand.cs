using MediatR;

namespace RU.Challenge.Domain.Commands
{
    public class CreateDistributionPlatformCommand : IRequest
    {
        public string Name { get; set; }

        public CreateDistributionPlatformCommand(string name)
            => Name = name;
    }
}