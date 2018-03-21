using Akka.TestKit.Xunit2;
using FluentAssertions;
using RU.Challenge.Domain.Commands;
using RU.Challenge.Domain.Events;
using RU.Challenge.Fixtures.Attributes;
using RU.Challenge.Fixtures.Helpers;
using System;
using Xunit;

namespace RU.Challenge.Infrastructure.Akka.Actors
{
    public class DistributionPlatformActorTests : TestKit
    {
        [Theory(DisplayName = "Distribution platform actor should execute CreateDistributionPlatformCommand successfully")]
        [DefaultData]
        public void DistributionPlatformActorShouldExecuteCreateDistributionPlatformCommandSuccessfully(
            Guid id,
            CreateDistributionPlatformCommand createDistributionPlatformCommand)
        {
            // Setup
            var probe = CreateTestProbe();
            var distributionPlatformActor = Sys.ActorOf(DistributionPlatformActor.GetProps(id));

            // Exercise
            distributionPlatformActor.Tell(createDistributionPlatformCommand, TestActor);

            // Verify outcome
            distributionPlatformActor.Tell("state", TestActor);
            var actual = ExpectMsg<Domain.Entities.DistributionPlatform>();
            actual.Id.Should().Be(id);
            actual.Name.Should().Be(createDistributionPlatformCommand.Name);
        }

        [Theory(DisplayName = "Distribution platform actor should recover successfully (journal)")]
        [DefaultData]
        public void DistributionPlatformActorShouldRecoverSuccessfullyJournal(
            Guid id,
            CreateDistributionPlatformCommand createDistributionPlatformCommand)
        {
            // Setup
            var probe = CreateTestProbe();
            PersistenceHelper.InitializeJournal(
                probe, id.ToString(), CreateDistributionPlatformEvent.CreateFromCommand(createDistributionPlatformCommand, id));

            // Exercise
            var distributionPlatformActor = Sys.ActorOf(DistributionPlatformActor.GetProps(id));

            // Verify outcome
            distributionPlatformActor.Tell("state", TestActor);
            var actual = ExpectMsg<Domain.Entities.DistributionPlatform>();
            actual.Id.Should().Be(id);
            actual.Name.Should().Be(createDistributionPlatformCommand.Name);
        }

        [Theory(DisplayName = "Distribution platform actor should recover successfully (snapshot)")]
        [DefaultData]
        public void DistributionPlatformActorShouldRecoverSuccessfullySnapshot(
            Domain.Entities.DistributionPlatform distributionPlatform)
        {
            // Setup
            var probe = CreateTestProbe();
            PersistenceHelper.InitializeSnapshot(probe, distributionPlatform.Id.ToString(), distributionPlatform);

            // Exercise
            var distributionPlatformActor = Sys.ActorOf(DistributionPlatformActor.GetProps(distributionPlatform.Id));

            // Verify outcome
            distributionPlatformActor.Tell("state", TestActor);
            var actual = ExpectMsg<Domain.Entities.DistributionPlatform>();
            actual.Id.Should().Be(distributionPlatform.Id);
            actual.Name.Should().Be(distributionPlatform.Name);
        }
    }
}