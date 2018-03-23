using MediatR;
using System;

namespace RU.Challenge.Domain.Commands
{
    public class CreatePaymentMethodCommand : IRequest
    {
        public Guid PaymentMethodId { get; set; }

        public string Name { get; set; }

        public CreatePaymentMethodCommand(string name)
            => Name = name;

        public CreatePaymentMethodCommand(Guid paymentMethodId, string name)
            : this(name)
        {
            PaymentMethodId = paymentMethodId;
        }
    }
}