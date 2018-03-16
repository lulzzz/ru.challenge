namespace RU.Challenge.Domain.Commands
{
    public class CreateArtistCommand
    {
        public int Age { get; private set; }

        public string Name { get; private set; }

        public CreateArtistCommand(int age, string name)
        {
            Age = age;
            Name = name;
        }
    }
}