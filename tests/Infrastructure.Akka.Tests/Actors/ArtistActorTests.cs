using Akka.TestKit.Xunit2;
using FluentAssertions;
using RU.Challenge.Domain.Commands;
using RU.Challenge.Fixtures.Attributes;
using RU.Challenge.Fixtures.Helpers;
using RU.Challenge.Infrastructure.Akka.Events;
using System;
using Xunit;

namespace RU.Challenge.Infrastructure.Akka.Actors
{
    public class ArtistActorTests : TestKit
    {
        [Theory(DisplayName = "Artist actor should execute CreateArtistCommand successfully")]
        [DefaultData]
        public void ArtistActorShouldExecuteCreateArtistCommandSuccessfully(
            Guid id,
            CreateArtistCommand createArtistCommand)
        {
            // Setup
            var artistActor = Sys.ActorOf(ArtistActor.GetProps(id));
            var probe = CreateTestProbe();

            // Exercise
            artistActor.Tell(createArtistCommand, TestActor);

            // Verify outcome
            Within(TimeSpan.FromSeconds(3), () =>
            {
                artistActor.Tell("state", TestActor);
                var actual = ExpectMsg<Domain.Entities.Artist>(TimeSpan.FromSeconds(10));
                actual.Id.Should().Be(id);
                actual.Age.Should().Be(createArtistCommand.Age);
                actual.Name.Should().Be(createArtistCommand.Name);
            });
        }

        [Theory(DisplayName = "Artist actor should recover successfully (journal)")]
        [DefaultData]
        public void ArtistActorShouldRecoverSuccessfullyJournal(
            Guid id,
            CreateArtistCommand createArtistCommand)
        {
            // Setup
            var probe = CreateTestProbe();
            PersistenceHelper.InitializeJournal(
                probe, id.ToString(), CreateArtistEvent.CreateFromCommand(createArtistCommand, id));

            // Exercise
            var artistActor = Sys.ActorOf(ArtistActor.GetProps(id));

            // Verify outcome
            artistActor.Tell("state", TestActor);
            var actual = ExpectMsg<Domain.Entities.Artist>(TimeSpan.FromSeconds(3));
            actual.Id.Should().Be(id);
            actual.Age.Should().Be(createArtistCommand.Age);
            actual.Name.Should().Be(createArtistCommand.Name);
        }

        [Theory(DisplayName = "Artist actor should recover successfully (snapshot)")]
        [DefaultData]
        public void ArtistActorShouldRecoverSuccessfullySnapshot(
            Domain.Entities.Artist artist)
        {
            // Setup
            var probe = CreateTestProbe();
            PersistenceHelper.InitializeSnapshot(probe, artist.Id.ToString(), artist);

            // Exercise
            var artistActor = Sys.ActorOf(ArtistActor.GetProps(artist.Id));

            // Verify outcome
            artistActor.Tell("state", TestActor);
            var actual = ExpectMsg<Domain.Entities.Artist>(TimeSpan.FromSeconds(3));
            actual.Id.Should().Be(artist.Id);
            actual.Age.Should().Be(artist.Age);
            actual.Name.Should().Be(artist.Name);
        }
    }
}