using Akka.Actor;
using Akka.Persistence;
using RU.Challenge.Domain.Commands;
using RU.Challenge.Domain.Events;
using RU.Challenge.Infrastructure.Akka.States;
using System;

namespace RU.Challenge.Infrastructure.Akka.Actors
{
    public class TrackActor : PersistentActor
    {
        private Guid _id;
        private TrackState _state;

        public TrackActor(Guid id)
            => _id = id;

        public override string PersistenceId => _id.ToString();

        protected override bool ReceiveCommand(object message)
        {
            switch (message)
            {
                case CreateTrackCommand createTrackCommand:
                    var createTrackEvent = CreateTrackEvent.CreateFromCommand(createTrackCommand);
                    Persist(createTrackEvent, CreateTrackEventHandler);
                    Context.System.EventStream.Publish(createTrackEvent);
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
                case CreateTrackEvent createTrackEvent:
                    CreateTrackEventHandler(createTrackEvent);
                    return true;

                case SnapshotOffer snapshotOffer:
                    _state = snapshotOffer.Snapshot as TrackState;
                    return true;
            }

            return true;
        }

        private void SnapshotCheck()
        {
            if (LastSequenceNr != 0 && LastSequenceNr % 10 == 0)
                SaveSnapshot(_state);
        }

        private void CreateTrackEventHandler(CreateTrackEvent createTrackEvent)
        {
            _state = new TrackState(
                createTrackEvent.Id,
                createTrackEvent.ReleaseId,
                createTrackEvent.Name,
                createTrackEvent.SongUrl,
                createTrackEvent.GenreId,
                createTrackEvent.ArtistId,
                createTrackEvent.Order);
        }

        public static Props GetProps(Guid id)
            => Props.Create(() => new TrackActor(id));
    }
}