using Akka.Actor;
using Akka.Persistence;
using RU.Challenge.Domain.Commands;
using RU.Challenge.Domain.Events;
using RU.Challenge.Infrastructure.Akka.States;
using System;
using System.Linq;

namespace RU.Challenge.Infrastructure.Akka.Actors
{
    public class ReleaseActor : PersistentActor
    {
        private Guid _id;
        private ReleaseState _state;

        public ReleaseActor(Guid id)
            => _id = id;

        public override string PersistenceId => _id.ToString();

        protected override bool ReceiveCommand(object message)
        {
            switch (message)
            {
                case CreateReleaseCommand createReleaseCommand:
                    var createReleaseEvent = CreateReleaseEvent.CreateFromCommand(createReleaseCommand);
                    Persist(createReleaseEvent, CreateReleaseEventHandler);
                    Context.System.EventStream.Publish(createReleaseEvent);
                    SnapshotCheck();
                    return true;

                case CreateTrackCommand createTrackCommand:

                    var order = _state.Tracks.Count();
                    createTrackCommand.SetOrder(order + 1);
                    var createTrackEvent = CreateTrackEvent.CreateFromCommand(createTrackCommand);

                    var trackActor = Context.ActorOf(TrackActor.GetProps(createTrackCommand.TrackId));
                    trackActor.Forward(createTrackEvent);

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
                case CreateReleaseEvent createReleaseEvent:
                    CreateReleaseEventHandler(createReleaseEvent);
                    return true;

                case SnapshotOffer snapshotOffer:
                    _state = snapshotOffer.Snapshot as ReleaseState;
                    return true;
            }

            return true;
        }

        private void SnapshotCheck()
        {
            if (LastSequenceNr != 0 && LastSequenceNr % 10 == 0)
                SaveSnapshot(_state);
        }

        private void CreateReleaseEventHandler(CreateReleaseEvent createReleaseEvent)
        {
            _state = new ReleaseState(
                createReleaseEvent.Id,
                createReleaseEvent.Title,
                createReleaseEvent.ArtistId,
                createReleaseEvent.GenreId,
                createReleaseEvent.CoverArtUrl,
                tracks: Enumerable.Empty<Guid>().ToList(),
                subscriptionId: default(Guid),
                userId: createReleaseEvent.UserId,
                status: Domain.Enums.ReleaseStatus.Created);
        }

        private void CreateTrackEventHandler(CreateTrackEvent createTrackEvent)
        {
            _state.Tracks.Add(createTrackEvent.Id);
        }

        public static Props GetProps(Guid id)
            => Props.Create(() => new ReleaseActor(id));
    }
}