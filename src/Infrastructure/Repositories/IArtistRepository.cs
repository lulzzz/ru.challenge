using RU.Challenge.Domain.Events;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IArtistRepository
    {
        Task AddAsync(CreateArtistEvent @event);
    }
}