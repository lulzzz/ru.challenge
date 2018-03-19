using RU.Challenge.Domain.Events;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IGenreRepository
    {
        Task AddAsync(CreateGenreEvent @event);
    }
}