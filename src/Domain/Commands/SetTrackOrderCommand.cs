namespace RU.Challenge.Domain.Commands
{
    public class SetTrackOrderCommand
    {
        public int Order { get; private set; }

        public SetTrackOrderCommand(int order)
            => Order = order;
    }
}