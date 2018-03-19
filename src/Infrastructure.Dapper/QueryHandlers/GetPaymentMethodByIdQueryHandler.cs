using Dapper;
using MediatR;
using RU.Challenge.Domain.Entities;
using RU.Challenge.Domain.Queries;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Dapper.QueryHandlers
{
    public class GetPaymentMethodByIdQueryHandler : IRequestHandler<GetPaymentMethodByIdQuery, PaymentMethod>
    {
        private readonly IDbConnection _dbConnection;

        public GetPaymentMethodByIdQueryHandler(IDbConnection dbConnection)
            => _dbConnection = dbConnection;

        public async Task<PaymentMethod> Handle(GetPaymentMethodByIdQuery request, CancellationToken cancellationToken)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<PaymentMethod>(
                sql: "SELECT * FROM payment_method WHERE id = @Id",
                param: new { request.Id });
        }
    }
}