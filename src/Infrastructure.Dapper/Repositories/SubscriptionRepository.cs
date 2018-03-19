using Dapper;
using Infrastructure.Repositories;
using RU.Challenge.Domain.Events;
using System;
using System.Data;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Dapper.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly IDbConnection _dbConnection;

        public SubscriptionRepository(IDbConnection dbConnection)
            => _dbConnection = dbConnection;

        public async Task AddAsync(CreateSubscriptionEvent @event)
        {
            var distributionPlatformsIds = string.Join(", ", @event.DistributionPlatformsIds);
            distributionPlatformsIds = "{ " + distributionPlatformsIds + " }";
            
            // TODO: This is not SQLi safe
            await _dbConnection.ExecuteAsync(
                $"INSERT INTO subscription (id, expiration_date, amount, payment_method_id, distribution_platforms_id) " +
                $"VALUES ('{@event.Id}', '{@event.ExpirationDate}', '{@event.Amount}', '{@event.PaymentMethodId}', '{distributionPlatformsIds}')");
        }

        public async Task AddDistributionPlatformAsync(AddDistributionPlatformToSubscriptionEvent @event)
        {
            // TODO: This is not SQLi safe
            await _dbConnection.ExecuteAsync(
                $"UPDATE subscription " +
                $"SET distribution_platforms_id = ARRAY_APPEND(distribution_platforms_id, '{@event.DistributionPlatformId}') " +
                $"WHERE id = '{@event.SubscriptionId}'");
        }
    }
}