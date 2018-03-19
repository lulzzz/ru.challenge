using Akka.Actor;
using Akka.Persistence;
using RU.Challenge.Domain.Commands;
using RU.Challenge.Domain.Events;
using RU.Challenge.Infrastructure.Akka.Snapshot;
using System;
using System.Linq;

namespace RU.Challenge.Infrastructure.Akka.Actors
{
    public class SubscriptionActor : PersistentActor
    {
        private Guid _id;
        private SubscriptionState _state;

        public SubscriptionActor(Guid id)
            => _id = id;

        public override string PersistenceId => _id.ToString();

        protected override bool ReceiveCommand(object message)
        {
            switch (message)
            {
                case CreateSubscriptionCommand createSubscriptionCommand:
                    var createSubscriptionEvent = CreateSubscriptionEvent.CreateFromCommand(createSubscriptionCommand, _id);
                    Persist(createSubscriptionEvent, CreateSubscriptionEventHandler);
                    Context.System.EventStream.Publish(createSubscriptionEvent);
                    SnapshotCheck();
                    return true;

                case AddDistributionPlatformToSubscriptionCommand addDistributionPlatformToSubscriptionCommand:
                    var addDistributionPlatformToSubscriptionEvent = AddDistributionPlatformToSubscriptionEvent.CreateFromCommand(addDistributionPlatformToSubscriptionCommand);
                    Persist(addDistributionPlatformToSubscriptionEvent, AddDistributionPlatformToSubscriptionEventHandler);
                    Context.System.EventStream.Publish(addDistributionPlatformToSubscriptionEvent);
                    SnapshotCheck();
                    return true;

                case RecoveryCompleted recoveryCompleted:
                    Log.Info($"Artist with ID {PersistenceId} recovery completed");
                    return true;
            }

            return true;
        }

        protected override bool ReceiveRecover(object message)
        {
            switch (message)
            {
                case CreateSubscriptionEvent createSubscriptionEvent:
                    CreateSubscriptionEventHandler(createSubscriptionEvent);
                    return true;

                case SnapshotOffer snapshotOffer:
                    _state = snapshotOffer.Snapshot as SubscriptionState;
                    return true;
            }

            return true;
        }

        private void SnapshotCheck()
        {
            if (LastSequenceNr != 0 && LastSequenceNr % 10 == 0)
                SaveSnapshot(_state);
        }

        private void CreateSubscriptionEventHandler(CreateSubscriptionEvent createSubscriptionEvent)
        {
            _state = new SubscriptionState(_id, createSubscriptionEvent.ExpirationDate, createSubscriptionEvent.Amount, createSubscriptionEvent.PaymentMethodId, createSubscriptionEvent.DistributionPlatformsIds.ToList());
        }

        private void AddDistributionPlatformToSubscriptionEventHandler(AddDistributionPlatformToSubscriptionEvent addDistributionPlatformToSubscriptionEvent)
        {
            _state.DistributionPlatformIds.Add(addDistributionPlatformToSubscriptionEvent.DistributionPlatformId);
        }

        public static Props GetProps(Guid id)
            => Props.Create(() => new SubscriptionActor(id));
    }
}