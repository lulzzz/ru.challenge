using MediatR;

namespace RU.Challenge.Domain.Commands
{
    public class CreatePaymentMethodCommand : IRequest
    {
        public string Name { get; set; }

        public CreatePaymentMethodCommand(string name)
            => Name = name;
    }
}