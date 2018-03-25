using Dapper;
using Infrastructure.Repositories;
using RU.Challenge.Domain.Events;
using System.Data;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Dapper.Repositories
{
    public class PaymentMethodRepository : IPaymentMethodRepository
    {
        private readonly IDbConnection _dbConnection;

        public PaymentMethodRepository(IDbConnection dbConnection)
            => _dbConnection = dbConnection ?? throw new System.ArgumentNullException(nameof(dbConnection));

        public async Task AddAsync(CreatePaymentMethodEvent @event)
        {
            await _dbConnection.ExecuteAsync(
                sql: $"INSERT INTO payment_method (id, name) VALUES (@Id, @Name)",
                param: new { @event.Id, @event.Name });
        }
    }
}