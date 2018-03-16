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
    public class GenreActorTests : TestKit
    {
        [Theory(DisplayName = "Genre actor should execute CreateGenreCommand successfully")]
        [DefaultData]
        public void GenreActorShouldExecuteCreateGenreCommandSuccessfully(
            Guid id,
            CreateGenreCommand createGenreCommand)
        {
            // Setup
            var genreActor = Sys.ActorOf(GenreActor.GetProps(id));
            var probe = CreateTestProbe();

            // Exercise
            genreActor.Tell(createGenreCommand, TestActor);

            // Verify outcome
            genreActor.Tell("state", TestActor);
            var actual = ExpectMsg<Domain.Entities.Genre>();
            actual.Id.Should().Be(id);
            actual.Name.Should().Be(createGenreCommand.Name);
        }

        [Theory(DisplayName = "Genre actor should recover successfully (journal)")]
        [DefaultData]
        public void GenreActorShouldRecoverSuccessfullyJournal(
            Guid id,
            CreateGenreCommand createGenreCommand)
        {
            // Setup
            var probe = CreateTestProbe();
            PersistenceHelper.InitializeJournal(
                probe, id.ToString(), CreateGenreEvent.CreateFromCommand(createGenreCommand, id));

            // Exercise
            var genreActor = Sys.ActorOf(GenreActor.GetProps(id));

            // Verify outcome
            genreActor.Tell("state", TestActor);
            var actual = ExpectMsg<Domain.Entities.Genre>();
            actual.Id.Should().Be(id);
            actual.Name.Should().Be(createGenreCommand.Name);
        }

        [Theory(DisplayName = "Genre actor should recover successfully (snapshot)")]
        [DefaultData]
        public void GenreActorShouldRecoverSuccessfullySnapshot(
            Domain.Entities.Genre genre)
        {
            // Setup
            var probe = CreateTestProbe();
            PersistenceHelper.InitializeSnapshot(probe, genre.Id.ToString(), genre);

            // Exercise
            var genreActor = Sys.ActorOf(GenreActor.GetProps(genre.Id));

            // Verify outcome
            genreActor.Tell("state", TestActor);
            var actual = ExpectMsg<Domain.Entities.Genre>();
            actual.Id.Should().Be(genre.Id);
            actual.Name.Should().Be(genre.Name);
        }
    }
}