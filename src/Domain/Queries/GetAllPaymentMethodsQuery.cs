using MediatR;
using System.Collections.Generic;

namespace RU.Challenge.Domain.Queries
{
    public class GetAllPaymentMethodsQuery : IRequest<IEnumerable<Entities.PaymentMethod>>
    {
    }
}