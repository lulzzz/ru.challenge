using RU.Challenge.Domain.Events;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface ITrackRepository
    {
        Task AddAsync(CreateTrackEvent @event);
    }
}