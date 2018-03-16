using Akka.Actor;
using Akka.Persistence;
using RU.Challenge.Domain.Commands;
using RU.Challenge.Infrastructure.Akka.Events;
using System;

namespace RU.Challenge.Infrastructure.Akka.Actors
{
    public class PaymentMethodActor : PersistentActor
    {
        private Guid _id;
        private Domain.Entities.PaymentMethod _state;

        public PaymentMethodActor(Guid id)
            => _id = id;

        public override string PersistenceId => _id.ToString();

        protected override bool ReceiveCommand(object message)
        {
            switch (message)
            {
                case CreatePaymentMethodCommand createPaymentMethodCommand:
                    var createArtistEvent = CreatePaymentMethodEvent.CreateFromCommand(createPaymentMethodCommand, _id);
                    Persist(createArtistEvent, CreatePaymentMethodEventHandler);
                    SnapshotCheck();
                    return true;

                case RecoveryCompleted recoveryCompleted:
                    Log.Info($"Artist with ID {PersistenceId} recovery completed");
                    return true;

                case "state":
                    Sender.Tell(_state);
                    return true;
            }

            return true;
        }

        protected override bool ReceiveRecover(object message)
        {
            switch (message)
            {
                case CreatePaymentMethodEvent createPaymentMethodEvent:
                    CreatePaymentMethodEventHandler(createPaymentMethodEvent);
                    return true;

                case SnapshotOffer snapshotOffer:
                    _state = snapshotOffer.Snapshot as Domain.Entities.PaymentMethod;
                    return true;
            }

            return true;
        }

        private void SnapshotCheck()
        {
            if (LastSequenceNr != 0 && LastSequenceNr % 10 == 0)
                SaveSnapshot(_state.Clone());
        }

        private void CreatePaymentMethodEventHandler(CreatePaymentMethodEvent createPaymentMethodEvent)
            => _state = Domain.Entities.PaymentMethod.Create(createPaymentMethodEvent.Id, createPaymentMethodEvent.Name);

        public static Props GetProps(Guid id)
            => Props.Create(() => new PaymentMethodActor(id));
    }
}