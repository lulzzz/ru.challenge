using RU.Challenge.Domain.Commands;
using System;

namespace RU.Challenge.Domain.Events
{
    public class CreatePaymentMethodEvent
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public CreatePaymentMethodEvent(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public static CreatePaymentMethodEvent CreateFromCommand(CreatePaymentMethodCommand command)
            => new CreatePaymentMethodEvent(command.PaymentMethodId, command.Name);
    }
}