namespace RU.Challenge.Domain.Commands
{
    public class CreatePaymentMethodCommand
    {
        public string Name { get; private set; }

        public CreatePaymentMethodCommand(string name)
        {
            Name = name;
        }
    }
}