using RU.Challenge.Domain.Events;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IDistributionPlatformRepository
    {
        Task AddAsync(CreateDistributionPlatformEvent @event);
    }
}