using Akka.Actor;
using Akka.Persistence;
using RU.Challenge.Domain.Commands;
using RU.Challenge.Domain.Events;
using System;

namespace RU.Challenge.Infrastructure.Akka.Actors
{
    public class GenreActor : PersistentActor
    {
        private Guid _id;
        private Domain.Entities.Genre _state;

        public GenreActor(Guid id)
            => _id = id;

        public override string PersistenceId => _id.ToString();

        protected override bool ReceiveCommand(object message)
        {
            switch (message)
            {
                case CreateGenreCommand createGenreCommand:
                    var createGenreEvent = CreateGenreEvent.CreateFromCommand(createGenreCommand);
                    Persist(createGenreEvent, CreateGenreEventHandler);
                    Context.System.EventStream.Publish(createGenreEvent);
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
                case CreateGenreEvent createGenreEvent:
                    CreateGenreEventHandler(createGenreEvent);
                    return true;

                case SnapshotOffer snapshotOffer:
                    _state = snapshotOffer.Snapshot as Domain.Entities.Genre;
                    return true;
            }

            return true;
        }

        private void SnapshotCheck()
        {
            if (LastSequenceNr != 0 && LastSequenceNr % 10 == 0)
                SaveSnapshot(_state.Clone());
        }

        private void CreateGenreEventHandler(CreateGenreEvent createGenreEvent)
            => _state = Domain.Entities.Genre.Create(createGenreEvent.Id, createGenreEvent.Name);

        public static Props GetProps(Guid id)
            => Props.Create(() => new GenreActor(id));
    }
}