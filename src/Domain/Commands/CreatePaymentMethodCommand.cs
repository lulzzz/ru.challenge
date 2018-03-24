using MediatR;
using System;

namespace RU.Challenge.Domain.Commands
{
    public class CreatePaymentMethodCommand : IRequest
    {
        public Guid PaymentMethodId { get; private set; }

        public string Name { get; set; }

        public CreatePaymentMethodCommand(string name)
            => Name = name;

        public void SetId(Guid paymentMethodId)
        {
            PaymentMethodId = paymentMethodId;
        }
    }
}