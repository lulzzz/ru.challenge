using MediatR;
using System;

namespace RU.Challenge.Domain.Queries
{
    public class GetPaymentMethodByIdQuery : IRequest<Entities.PaymentMethod>
    {
        public Guid Id { get; set; }

        public GetPaymentMethodByIdQuery(Guid id)
            => Id = id;
    }
}