using RU.Challenge.Domain.Events;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IPaymentMethodRepository
    {
        Task AddAsync(CreatePaymentMethodEvent @event);
    }
}