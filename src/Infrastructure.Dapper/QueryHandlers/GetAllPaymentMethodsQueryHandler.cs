using Dapper;
using MediatR;
using RU.Challenge.Domain.Entities;
using RU.Challenge.Domain.Queries;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Dapper.QueryHandlers
{
    public class GetAllPaymentMethodsQueryHandler : IRequestHandler<GetAllPaymentMethodsQuery, IEnumerable<PaymentMethod>>
    {
        private readonly IDbConnection _dbConnection;

        public GetAllPaymentMethodsQueryHandler(IDbConnection dbConnection)
            => _dbConnection = dbConnection;

        public async Task<IEnumerable<PaymentMethod>> Handle(GetAllPaymentMethodsQuery request, CancellationToken cancellationToken)
        {
            return await _dbConnection.QueryAsync<PaymentMethod>(sql: "SELECT * FROM payment_method");
        }
    }
}