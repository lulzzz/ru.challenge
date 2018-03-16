namespace RU.Challenge.Domain.Commands
{
    public class CreateGenreCommand
    {
        public string Name { get; private set; }

        public CreateGenreCommand(string name)
        {
            Name = name;
        }
    }
}