using MediatR;

namespace RU.Challenge.Domain.Commands
{
    public class SetTrackOrderCommand : IRequest
    {
        public int Order { get; set; }

        public SetTrackOrderCommand(int order)
            => Order = order;
    }
}