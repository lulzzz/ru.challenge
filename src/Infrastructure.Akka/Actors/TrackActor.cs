using Akka.Actor;
using Akka.Persistence;
using RU.Challenge.Domain.Commands;
using RU.Challenge.Infrastructure.Akka.Events;
using RU.Challenge.Infrastructure.Akka.Snapshot;
using System;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Akka.Actors
{
    public class TrackActor : PersistentActor
    {
        private Guid _id;
        private Domain.Entities.Track _state;

        private Guid _genreId;
        private Guid _artistId;

        public TrackActor(Guid id)
            => _id = id;

        public override string PersistenceId => _id.ToString();

        protected override bool ReceiveCommand(object message)
        {
            switch (message)
            {
                case CreateTrackCommand createTrackCommand:
                    var createTrackEvent = CreateTrackEvent.CreateFromCommand(createTrackCommand, _id);
                    Persist(createTrackEvent, CreateTrackEventHandler);
                    SnapshotCheck();
                    return true;

                case SetTrackOrderCommand setTrackOrderCommand:
                    var setTrackOrderEvent = SetTrackOrderEvent.CreateFromCommand(setTrackOrderCommand, _id);
                    Persist(setTrackOrderEvent, SetTrackOrderEventHandler);
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
                case CreateTrackEvent createTrackEvent:
                    CreateTrackEventHandler(createTrackEvent);
                    return true;

                case SnapshotOffer snapshotOffer:
                    var snapshot = snapshotOffer.Snapshot as TrackAggregateSnapshot;
                    _artistId = snapshot.ArtistId;
                    _genreId = snapshot.GenreId;
                    _state = Domain.Entities.Track.Create(snapshot.Id, snapshot.Name, snapshot.SongUrl);
                    _state.SetOrder(snapshot.Order);
                    return true;
            }

            return true;
        }

        private void SnapshotCheck()
        {
            if (LastSequenceNr != 0 && LastSequenceNr % 10 == 0)
                SaveSnapshot(new TrackAggregateSnapshot(_id, _state.Name, _artistId, _genreId, _state.Order, _state.SongUrl));
        }

        private void CreateTrackEventHandler(CreateTrackEvent createTrackEvent)
        {
            _genreId = createTrackEvent.GenreId;
            _artistId = createTrackEvent.ArtistId;
            _state = Domain.Entities.Track.Create(createTrackEvent.Id, createTrackEvent.Name, createTrackEvent.SongUrl);
        }

        private void SetTrackOrderEventHandler(SetTrackOrderEvent setTrackOrderEvent)
        {
            _state.SetOrder(setTrackOrderEvent.Order);
        }

        private void PopulateDependencies()
        {
            var tGenre = Context.ActorOf(GenreActor.GetProps(_genreId)).Ask<Domain.Entities.Genre>("state");
            var tArtist = Context.ActorOf(ArtistActor.GetProps(_artistId)).Ask<Domain.Entities.Artist>("state");
            Task.WhenAll(tGenre, tArtist).GetAwaiter().GetResult();

            _state.SetGenre(tGenre.Result);
            _state.SetArtist(tArtist.Result);
        }

        public static Props GetProps(Guid id)
            => Props.Create(() => new TrackActor(id));
    }
}