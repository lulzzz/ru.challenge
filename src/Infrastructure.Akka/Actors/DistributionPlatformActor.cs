using Akka.Actor;
using Akka.Persistence;
using RU.Challenge.Domain.Commands;
using RU.Challenge.Domain.Events;
using System;

namespace RU.Challenge.Infrastructure.Akka.Actors
{
    public class DistributionPlatformActor : PersistentActor
    {
        private Guid _id;
        private Domain.Entities.DistributionPlatform _state;

        public DistributionPlatformActor(Guid id)
            => _id = id;

        public override string PersistenceId => _id.ToString();

        protected override bool ReceiveCommand(object message)
        {
            switch (message)
            {
                case CreateDistributionPlatformCommand createDistributionPlatformCommand:
                    var createDistributionPlatformEvent = CreateDistributionPlatformEvent.CreateFromCommand(createDistributionPlatformCommand, _id);
                    Persist(createDistributionPlatformEvent, CreateDistributionPlatformEventHandler);
                    Context.System.EventStream.Publish(createDistributionPlatformEvent);
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
                case CreateDistributionPlatformEvent createDistributionPlatformEvent:
                    CreateDistributionPlatformEventHandler(createDistributionPlatformEvent);
                    return true;

                case SnapshotOffer snapshotOffer:
                    _state = snapshotOffer.Snapshot as Domain.Entities.DistributionPlatform;
                    return true;
            }

            return true;
        }

        private void SnapshotCheck()
        {
            if (LastSequenceNr != 0 && LastSequenceNr % 10 == 0)
                SaveSnapshot(_state.Clone());
        }

        private void CreateDistributionPlatformEventHandler(CreateDistributionPlatformEvent createDistributionPlatformEvent)
            => _state = Domain.Entities.DistributionPlatform.Create(createDistributionPlatformEvent.Id, createDistributionPlatformEvent.Name);

        public static Props GetProps(Guid id)
            => Props.Create(() => new DistributionPlatformActor(id));
    }
}