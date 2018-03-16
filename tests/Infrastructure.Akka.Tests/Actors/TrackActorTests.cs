using Akka.TestKit.Xunit2;
using FluentAssertions;
using RU.Challenge.Domain.Commands;
using RU.Challenge.Fixtures.Attributes;
using RU.Challenge.Fixtures.Helpers;
using RU.Challenge.Infrastructure.Akka.Events;
using RU.Challenge.Infrastructure.Akka.Snapshot;
using System;
using Xunit;

namespace RU.Challenge.Infrastructure.Akka.Actors
{
    public class TrackActorTests : TestKit
    {
        [Theory(DisplayName = "Track actor should execute CreateTrackCommand successfully")]
        [DefaultData]
        public void TrackActorShouldExecuteCreateTrackCommandSuccessfully(
            Guid id,
            CreateGenreCommand createGenreCommand,
            CreateArtistCommand createArtistCommand,
            CreateTrackCommand createTrackCommand)
        {
            // Setup
            var probe = CreateTestProbe();
            CreateRelatedArtistAndGenre(createGenreCommand, createArtistCommand, createTrackCommand);

            // Exercise
            var trackActor = Sys.ActorOf(TrackActor.GetProps(id));
            trackActor.Tell(createTrackCommand, TestActor);

            // Verify outcome
            trackActor.Tell("state", TestActor);
            var actual = ExpectMsg<Domain.Entities.Track>();
            actual.Id.Should().Be(id);
            actual.Name.Should().Be(createTrackCommand.Name);
            actual.SongUrl.Should().Be(createTrackCommand.SongUrl);
            actual.Genre.Id.Should().Be(createTrackCommand.GenreId);
            actual.Genre.Name.Should().Be(createGenreCommand.Name);
            actual.Artist.Id.Should().Be(createTrackCommand.ArtistId);
            actual.Artist.Name.Should().Be(createArtistCommand.Name);
        }

        [Theory(DisplayName = "Track actor should recover successfully (journal)")]
        [DefaultData]
        public void TrackActorShouldRecoverSuccessfullyJournal(
            Guid id,
            CreateGenreCommand createGenreCommand,
            CreateArtistCommand createArtistCommand,
            CreateTrackCommand createTrackCommand)
        {
            // Setup
            var probe = CreateTestProbe();
            CreateRelatedArtistAndGenre(createGenreCommand, createArtistCommand, createTrackCommand);

            PersistenceHelper.InitializeJournal(
                probe, id.ToString(), CreateTrackEvent.CreateFromCommand(createTrackCommand, id));

            // Exercise
            var trackActor = Sys.ActorOf(TrackActor.GetProps(id));

            // Verify outcome
            trackActor.Tell("state", TestActor);
            var actual = ExpectMsg<Domain.Entities.Track>();
            actual.Id.Should().Be(id);
            actual.Name.Should().Be(createTrackCommand.Name);
            actual.SongUrl.Should().Be(createTrackCommand.SongUrl);
            actual.Genre.Id.Should().Be(createTrackCommand.GenreId);
            actual.Genre.Name.Should().Be(createGenreCommand.Name);
            actual.Artist.Id.Should().Be(createTrackCommand.ArtistId);
            actual.Artist.Name.Should().Be(createArtistCommand.Name);
        }

        [Theory(DisplayName = "Track actor should recover successfully (snapshot)")]
        [DefaultData]
        public void TrackActorShouldRecoverSuccessfullySnapshot(
            CreateGenreCommand createGenreCommand,
            CreateArtistCommand createArtistCommand,
            TrackAggregateSnapshot trackAggregateSnapshot)
        {
            // Setup
            var probe = CreateTestProbe();
            CreateRelatedArtistAndGenre(createGenreCommand, createArtistCommand, trackAggregateSnapshot);

            PersistenceHelper.InitializeSnapshot(probe, trackAggregateSnapshot.Id.ToString(), trackAggregateSnapshot);

            // Exercise
            var trackActor = Sys.ActorOf(TrackActor.GetProps(trackAggregateSnapshot.Id));

            // Verify outcome
            trackActor.Tell("state", TestActor);
            var actual = ExpectMsg<Domain.Entities.Track>();
            actual.Id.Should().Be(trackAggregateSnapshot.Id);
            actual.Name.Should().Be(trackAggregateSnapshot.Name);
            actual.SongUrl.Should().Be(trackAggregateSnapshot.SongUrl);
            actual.Genre.Id.Should().Be(trackAggregateSnapshot.GenreId);
            actual.Genre.Name.Should().Be(createGenreCommand.Name);
            actual.Artist.Id.Should().Be(trackAggregateSnapshot.ArtistId);
            actual.Artist.Name.Should().Be(createArtistCommand.Name);
        }

        private void CreateRelatedArtistAndGenre(
            CreateGenreCommand createGenreCommand,
            CreateArtistCommand createArtistCommand,
            CreateTrackCommand createTrackCommand)
        {
            Sys.ActorOf(ArtistActor.GetProps(createTrackCommand.ArtistId)).Tell(createArtistCommand, TestActor);
            Sys.ActorOf(GenreActor.GetProps(createTrackCommand.GenreId)).Tell(createGenreCommand, TestActor);
        }

        private void CreateRelatedArtistAndGenre(
            CreateGenreCommand createGenreCommand,
            CreateArtistCommand createArtistCommand,
            TrackAggregateSnapshot trackAggregateSnapshot)
        {
            Sys.ActorOf(ArtistActor.GetProps(trackAggregateSnapshot.ArtistId)).Tell(createArtistCommand, TestActor);
            Sys.ActorOf(GenreActor.GetProps(trackAggregateSnapshot.GenreId)).Tell(createGenreCommand, TestActor);
        }
    }
}