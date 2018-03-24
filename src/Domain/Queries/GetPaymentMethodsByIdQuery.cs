using MediatR;
using System;
using System.Collections.Generic;

namespace RU.Challenge.Domain.Queries
{
    public class GetPaymentMethodsByIdQuery : IRequest<IEnumerable<Entities.PaymentMethod>>
    {
        public IEnumerable<Guid> Ids { get; set; }

        public GetPaymentMethodsByIdQuery(IEnumerable<Guid> ids)
            => Ids = ids;
    }
}