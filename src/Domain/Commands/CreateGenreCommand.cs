using MediatR;

namespace RU.Challenge.Domain.Commands
{
    public class CreateGenreCommand : IRequest
    {
        public string Name { get; set; }

        public CreateGenreCommand(string name)
            => Name = name;
    }
}