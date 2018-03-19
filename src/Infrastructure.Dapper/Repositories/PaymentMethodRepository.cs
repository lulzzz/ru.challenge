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
            => _dbConnection = dbConnection;

        public async Task AddAsync(CreatePaymentMethodEvent @event)
        {
            // TODO: This is not SQLi safe
            await _dbConnection.ExecuteAsync($"INSERT INTO payment_method (id, name) VALUES ('{@event.Id}', '{@event.Name}')");
        }
    }
}