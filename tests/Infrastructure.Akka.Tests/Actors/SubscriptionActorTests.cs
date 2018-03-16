using Akka.TestKit.Xunit2;
using FluentAssertions;
using RU.Challenge.Domain.Commands;
using RU.Challenge.Fixtures.Attributes;
using RU.Challenge.Fixtures.Helpers;
using RU.Challenge.Infrastructure.Akka.Events;
using RU.Challenge.Infrastructure.Akka.Snapshot;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RU.Challenge.Infrastructure.Akka.Actors
{
    public class SubscriptionActorTests : TestKit
    {
        [Theory(DisplayName = "Subscription actor should execute CreateSubscriptionCommand successfully")]
        [DefaultData]
        public void SubscriptionActorShouldExecuteCreateSubscriptionCommandSuccessfully(
            Guid id,
            CreatePaymentMethodCommand createPaymentMethodCommand,
            IEnumerable<CreateDistributionPlatformCommand> createDistributionPlatformCommands,
            CreateSubscriptionCommand createSubscriptionCommand)
        {
            // Setup
            var probe = CreateTestProbe();

            CreateRelatedPaymentAndDistributionPlatforms(
                createPaymentMethodCommand,
                createDistributionPlatformCommands,
                createSubscriptionCommand);

            // Exercise
            var subscriptionActor = Sys.ActorOf(SubscriptionActor.GetProps(id));
            subscriptionActor.Tell(createSubscriptionCommand, TestActor);

            // Verify outcome
            subscriptionActor.Tell("state", TestActor);
            var actual = ExpectMsg<Domain.Entities.Subscription>();
            actual.Id.Should().Be(id);
            actual.Amount.Should().Be(createSubscriptionCommand.Amount);
            actual.ExpirationDate.Should().Be(createSubscriptionCommand.ExpirationDate);
            actual.PaymentMethod.Id.Should().Be(createSubscriptionCommand.PaymentMethodId);
            actual.PaymentMethod.Name.Should().Be(createPaymentMethodCommand.Name);
            foreach (var actDp in actual.DistributionPlatforms)
            {
                createSubscriptionCommand.DistributionPlatformIds.Should().Contain(actDp.Id);
                createDistributionPlatformCommands.Should().ContainSingle(e => e.Name == actDp.Name);
            }
        }

        [Theory(DisplayName = "Subscription actor should recover successfully (journal)")]
        [DefaultData]
        public void SubscriptionActorShouldRecoverSuccessfullyJournal(
            Guid id,
            CreatePaymentMethodCommand createPaymentMethodCommand,
            IEnumerable<CreateDistributionPlatformCommand> createDistributionPlatformCommands,
            CreateSubscriptionCommand createSubscriptionCommand)
        {
            // Setup
            var probe = CreateTestProbe();

            CreateRelatedPaymentAndDistributionPlatforms(
                createPaymentMethodCommand,
                createDistributionPlatformCommands,
                createSubscriptionCommand);

            PersistenceHelper.InitializeJournal(
                probe, id.ToString(), CreateSubscriptionEvent.CreateFromCommand(createSubscriptionCommand, id));

            // Exercise
            var subscriptionActor = Sys.ActorOf(SubscriptionActor.GetProps(id));

            // Verify outcome
            subscriptionActor.Tell("state", TestActor);
            var actual = ExpectMsg<Domain.Entities.Subscription>();
            actual.Id.Should().Be(id);
            actual.Amount.Should().Be(createSubscriptionCommand.Amount);
            actual.ExpirationDate.Should().Be(createSubscriptionCommand.ExpirationDate);
            actual.PaymentMethod.Id.Should().Be(createSubscriptionCommand.PaymentMethodId);
            actual.PaymentMethod.Name.Should().Be(createPaymentMethodCommand.Name);
            foreach (var actDp in actual.DistributionPlatforms)
            {
                createSubscriptionCommand.DistributionPlatformIds.Should().Contain(actDp.Id);
                createDistributionPlatformCommands.Should().ContainSingle(e => e.Name == actDp.Name);
            }
        }

        [Theory(DisplayName = "Subscription actor should recover successfully (snapshot)")]
        [DefaultData]
        public void SubscriptionActorShouldRecoverSuccessfullySnapshot(
            CreatePaymentMethodCommand createPaymentMethodCommand,
            IEnumerable<CreateDistributionPlatformCommand> createDistributionPlatformCommands,
            SubscriptionAggregateSnapshot subscriptionSnapshot)
        {
            // Setup
            var probe = CreateTestProbe();

            CreateRelatedPaymentAndDistributionPlatforms(
                createPaymentMethodCommand,
                createDistributionPlatformCommands,
                subscriptionSnapshot);

            PersistenceHelper.InitializeSnapshot(probe, subscriptionSnapshot.Id.ToString(), subscriptionSnapshot);

            // Exercise
            var subscriptionActor = Sys.ActorOf(SubscriptionActor.GetProps(subscriptionSnapshot.Id));

            // Verify outcome
            subscriptionActor.Tell("state", TestActor);
            var actual = ExpectMsg<Domain.Entities.Subscription>();
            actual.Id.Should().Be(subscriptionSnapshot.Id);
            actual.Amount.Should().Be(subscriptionSnapshot.Amount);
            actual.ExpirationDate.Should().Be(subscriptionSnapshot.ExpirationDate);
            actual.PaymentMethod.Id.Should().Be(subscriptionSnapshot.PaymentMethodId);
            actual.PaymentMethod.Name.Should().Be(createPaymentMethodCommand.Name);
            foreach (var actDp in actual.DistributionPlatforms)
            {
                subscriptionSnapshot.DistributionPlatformIds.Should().Contain(actDp.Id);
                createDistributionPlatformCommands.Should().ContainSingle(e => e.Name == actDp.Name);
            }
        }

        private void CreateRelatedPaymentAndDistributionPlatforms(
            CreatePaymentMethodCommand createPaymentMethodCommand,
            IEnumerable<CreateDistributionPlatformCommand> createDistributionPlatformCommands,
            CreateSubscriptionCommand createSubscriptionCommand)
        {
            // Create Payment methods (pre condition)
            var paymentId = createSubscriptionCommand.PaymentMethodId;

            Sys.ActorOf(PaymentMethodActor.GetProps(paymentId))
               .Tell(createPaymentMethodCommand, TestActor);

            // Create Distribution platforms (pre condition)
            for (int i = 0; i < createSubscriptionCommand.DistributionPlatformIds.Count(); i++)
            {
                var createDistributionPlatformCommand = createDistributionPlatformCommands.ElementAt(i);

                Sys.ActorOf(DistributionPlatformActor.GetProps(createSubscriptionCommand.DistributionPlatformIds.ElementAt(i)))
                   .Tell(createDistributionPlatformCommand, TestActor);
            }
        }

        private void CreateRelatedPaymentAndDistributionPlatforms(
            CreatePaymentMethodCommand createPaymentMethodCommand,
            IEnumerable<CreateDistributionPlatformCommand> createDistributionPlatformCommands,
            SubscriptionAggregateSnapshot subscriptionAggregateSnapshot)
        {
            // Create Payment methods (pre condition)
            var paymentId = subscriptionAggregateSnapshot.PaymentMethodId;

            Sys.ActorOf(PaymentMethodActor.GetProps(paymentId))
               .Tell(createPaymentMethodCommand, TestActor);

            //Create Distribution platforms(pre condition)
            for (int i = 0; i < subscriptionAggregateSnapshot.DistributionPlatformIds.Count(); i++)
            {
                var createDistributionPlatformCommand = createDistributionPlatformCommands.ElementAt(i);

                Sys.ActorOf(DistributionPlatformActor.GetProps(subscriptionAggregateSnapshot.DistributionPlatformIds.ElementAt(i)))
                   .Tell(createDistributionPlatformCommand, TestActor);
            }
        }
    }
}