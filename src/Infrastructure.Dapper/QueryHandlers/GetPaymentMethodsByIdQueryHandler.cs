using Dapper;
using MediatR;
using RU.Challenge.Domain.Entities;
using RU.Challenge.Domain.Queries;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Dapper.QueryHandlers
{
    public class GetPaymentMethodsByIdQueryHandler : IRequestHandler<GetPaymentMethodsByIdQuery, IEnumerable<PaymentMethod>>
    {
        private readonly IDbConnection _dbConnection;

        public GetPaymentMethodsByIdQueryHandler(IDbConnection dbConnection)
            => _dbConnection = dbConnection ?? throw new System.ArgumentNullException(nameof(dbConnection));

        public async Task<IEnumerable<PaymentMethod>> Handle(GetPaymentMethodsByIdQuery request, CancellationToken cancellationToken)
        {
            return await _dbConnection.QueryAsync<PaymentMethod>(
                sql: $"SELECT * FROM payment_method WHERE @Filter IS false OR id = ANY (@Ids)",
                param: new
                {
                    Filter = request.Ids != null && request.Ids.Any(),
                    Ids = request.Ids != null ? request.Ids.ToList() : request.Ids
                });
        }
    }
}