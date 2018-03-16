using Akka.Actor;
using Akka.Persistence;
using Akka.TestKit;
using System;

namespace RU.Challenge.Fixtures.Helpers
{
    public static class PersistenceHelper
    {
        public static void InitializeJournal(TestProbe probe, string persistenceId, params object[] events)
        {
            var writerGuid = Guid.NewGuid().ToString();
            var writes = new AtomicWrite[events.Length];
            for (int i = 0; i < events.Length; i++)
            {
                var e = events[i];
                writes[i] = new AtomicWrite(new Persistent(e, i + 1, persistenceId, "", false, ActorRefs.NoSender, writerGuid));
            }
            var journal = Persistence.Instance.Apply(probe.Sys).JournalFor(null);
            journal.Tell(new WriteMessages(writes, probe.Ref, 1));

            probe.ExpectMsg<WriteMessagesSuccessful>();
            for (int i = 0; i < events.Length; i++)
                probe.ExpectMsg<WriteMessageSuccess>();
        }

        public static void InitializeSnapshot(TestProbe probe, string persistenceId, params object[] snapshots)
        {
            var snapshot = Persistence.Instance.Apply(probe.Sys).SnapshotStoreFor(null);

            for (int i = 0; i < snapshots.Length; i++)
            {
                var e = snapshots[i];
                snapshot.Tell(new SaveSnapshot(new SnapshotMetadata(persistenceId, i + 1, DateTime.Now), snapshots[i]), probe.Ref);
                probe.ExpectMsg<SaveSnapshotSuccess>();
            }
        }
    }
}