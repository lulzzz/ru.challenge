using RU.Challenge.Domain.Commands;
using System;

namespace RU.Challenge.Domain.Events
{
    public class SetTrackOrderEvent
    {
        public Guid Id { get; private set; }

        public int Order { get; private set; }

        public SetTrackOrderEvent(Guid id, int order)
        {
            Id = id;
            Order = order;
        }

        public static SetTrackOrderEvent CreateFromCommand(SetTrackOrderCommand command, Guid id)
            => new SetTrackOrderEvent(id, command.Order);
    }
}