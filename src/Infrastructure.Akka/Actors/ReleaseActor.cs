using Akka.Actor;
using Akka.Persistence;
using RU.Challenge.Domain.Commands;
using RU.Challenge.Domain.Enums;
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

                    if (_state.Status == ReleaseStatus.Published)
                        return false;

                    var order = _state.Tracks.Count();
                    createTrackCommand.SetOrder(order + 1);
                    var createTrackEvent = CreateTrackEvent.CreateFromCommand(createTrackCommand);

                    var trackActor = Context.ActorOf(TrackActor.GetProps(createTrackCommand.TrackId));
                    trackActor.Forward(createTrackCommand);

                    Persist(createTrackEvent, CreateTrackEventHandler);
                    SnapshotCheck();
                    return true;

                case AddSubscriptionToReleaseCommand addSubscriptionToReleaseCommand:
                    var addSubscriptionToReleaseEvent = AddSubscriptionToReleaseEvent.CreateFromCommand(addSubscriptionToReleaseCommand);
                    Persist(addSubscriptionToReleaseEvent, AddSubscriptionToReleaseEventHandler);
                    Context.System.EventStream.Publish(addSubscriptionToReleaseEvent);
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

                case CreateTrackEvent createTrackEvent:
                    CreateTrackEventHandler(createTrackEvent);
                    return true;

                case AddSubscriptionToReleaseEvent addSubscriptionToReleaseEvent:
                    AddSubscriptionToReleaseEventHandler(addSubscriptionToReleaseEvent);
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

        private void AddSubscriptionToReleaseEventHandler(AddSubscriptionToReleaseEvent addSubscriptionToReleaseEvent)
        {
            _state.Status = ReleaseStatus.Published;
            _state.SubscriptionId = addSubscriptionToReleaseEvent.SubscriptionId;
        }

        public static Props GetProps(Guid id)
            => Props.Create(() => new ReleaseActor(id));
    }
}