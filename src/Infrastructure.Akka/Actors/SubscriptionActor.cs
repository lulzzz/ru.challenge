using Akka.Actor;
using Akka.Persistence;
using RU.Challenge.Domain.Commands;
using RU.Challenge.Infrastructure.Akka.Events;
using RU.Challenge.Infrastructure.Akka.Snapshot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Akka.Actors
{
    public class SubscriptionActor : PersistentActor
    {
        private Guid _id;
        private Domain.Entities.Subscription _state;

        private Guid _paymentMethodId;
        private List<Guid> _distributionPlatformsId;

        public SubscriptionActor(Guid id)
        {
            _id = id;
            _distributionPlatformsId = new List<Guid>();
        }

        public override string PersistenceId => _id.ToString();

        protected override bool ReceiveCommand(object message)
        {
            switch (message)
            {
                case CreateSubscriptionCommand createSubscriptionCommand:
                    var createSubscriptionEvent = CreateSubscriptionEvent.CreateFromCommand(createSubscriptionCommand, _id);
                    Persist(createSubscriptionEvent, CreateSubscriptionEventHandler);
                    SnapshotCheck();
                    return true;

                case RecoveryCompleted recoveryCompleted:
                    Log.Info($"Artist with ID {PersistenceId} recovery completed");
                    return true;

                case "state":
                    PopulateDependencies();
                    Sender.Tell(_state);
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
                    var snapshot = snapshotOffer.Snapshot as SubscriptionAggregateSnapshot;
                    _paymentMethodId = snapshot.PaymentMethodId;
                    _distributionPlatformsId = snapshot.DistributionPlatformIds.ToList();
                    _state = Domain.Entities.Subscription.Create(snapshot.Id, snapshot.ExpirationDate, snapshot.Amount);
                    return true;
            }

            return true;
        }

        private void SnapshotCheck()
        {
            if (LastSequenceNr != 0 && LastSequenceNr % 10 == 0)
                SaveSnapshot(new SubscriptionAggregateSnapshot(
                    _state.Id, _state.ExpirationDate, _state.Amount, _paymentMethodId, _distributionPlatformsId));
        }

        private void CreateSubscriptionEventHandler(CreateSubscriptionEvent createSubscriptionEvent)
        {
            _paymentMethodId = createSubscriptionEvent.PaymentMethodId;
            _distributionPlatformsId = createSubscriptionEvent.DistributionPlatformsIds.ToList();
            _state = Domain.Entities.Subscription.Create(createSubscriptionEvent.Id, createSubscriptionEvent.ExpirationDate, createSubscriptionEvent.Amount);
        }

        private void PopulateDependencies()
        {
            var tPayment = Context.ActorOf(PaymentMethodActor.GetProps(_paymentMethodId)).Ask<Domain.Entities.PaymentMethod>("state");
            var tPlatforms = _distributionPlatformsId.Select(e => Context.ActorOf(DistributionPlatformActor.GetProps(e)).Ask<Domain.Entities.DistributionPlatform>("state"));

            Task.WhenAll(new Task[] { tPayment, }.Concat(tPlatforms)).GetAwaiter().GetResult();

            _state.SetPaymentMethod(tPayment.Result);
            foreach (var tPlat in tPlatforms)
                _state.AddDistributionPlatform(tPlat.Result);
        }

        public static Props GetProps(Guid id)
            => Props.Create(() => new SubscriptionActor(id));
    }
}