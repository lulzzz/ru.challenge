using RU.Challenge.Domain.Events;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IReleaseRepository
    {
        Task AddAsync(CreateReleaseEvent @event);

        Task AddSubscriptionAsync(AddSubscriptionToReleaseEvent @event);
    }
}