using RU.Challenge.Domain.Events;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface ISubscriptionRepository
    {
        Task AddAsync(CreateSubscriptionEvent @event);

        Task AddDistributionPlatformAsync(AddDistributionPlatformToSubscriptionEvent @event);
    }
}