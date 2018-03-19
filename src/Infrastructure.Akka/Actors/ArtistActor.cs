using Akka.Actor;
using Akka.Persistence;
using RU.Challenge.Domain.Commands;
using RU.Challenge.Domain.Events;
using System;

namespace RU.Challenge.Infrastructure.Akka.Actors
{
    public class ArtistActor : PersistentActor
    {
        private Guid _id;
        private Domain.Entities.Artist _state;

        public ArtistActor(Guid id)
            => _id = id;

        public override string PersistenceId => _id.ToString();

        protected override bool ReceiveCommand(object message)
        {
            switch (message)
            {
                case CreateArtistCommand createArtistCommand:
                    var createArtistEvent = CreateArtistEvent.CreateFromCommand(createArtistCommand, _id);
                    Persist(createArtistEvent, CreateArtistEventHandler);
                    Context.System.EventStream.Publish(createArtistEvent);
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
                case CreateArtistEvent createArtistEvent:
                    CreateArtistEventHandler(createArtistEvent);
                    return true;

                case SnapshotOffer snapshotOffer:
                    _state = snapshotOffer.Snapshot as Domain.Entities.Artist;
                    return true;
            }

            return true;
        }

        private void SnapshotCheck()
        {
            if (LastSequenceNr != 0 && LastSequenceNr % 10 == 0)
                SaveSnapshot(_state.Clone());
        }

        private void CreateArtistEventHandler(CreateArtistEvent createArtistEvent)
            => _state = Domain.Entities.Artist.Create(createArtistEvent.Id, createArtistEvent.Age, createArtistEvent.Name);

        public static Props GetProps(Guid id)
            => Props.Create(() => new ArtistActor(id));
    }
}