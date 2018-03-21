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
    public class PaymentMethodActorTests : TestKit
    {
        [Theory(DisplayName = "Payment method actor should execute CreatePaymentMethodCommand successfully")]
        [DefaultData]
        public void PaymentMethodActorShouldExecuteCreatePaymentMethodCommandSuccessfully(
            Guid id,
            CreatePaymentMethodCommand createPaymentMethodCommand)
        {
            // Setup
            var probe = CreateTestProbe();
            var paymentMethodActor = Sys.ActorOf(PaymentMethodActor.GetProps(id));

            // Exercise
            paymentMethodActor.Tell(createPaymentMethodCommand, TestActor);

            // Verify outcome
            paymentMethodActor.Tell("state", TestActor);
            var actual = ExpectMsg<Domain.Entities.PaymentMethod>();
            actual.Id.Should().Be(id);
            actual.Name.Should().Be(createPaymentMethodCommand.Name);
        }

        [Theory(DisplayName = "Payment method actor should recover successfully (journal)")]
        [DefaultData]
        public void PaymentMethodActorShouldRecoverSuccessfullyJournal(
            Guid id,
            CreatePaymentMethodCommand createPaymentMethodCommand)
        {
            // Setup
            var probe = CreateTestProbe();
            PersistenceHelper.InitializeJournal(
                probe, id.ToString(), CreatePaymentMethodEvent.CreateFromCommand(createPaymentMethodCommand, id));

            // Exercise
            var paymentMethodActor = Sys.ActorOf(PaymentMethodActor.GetProps(id));

            // Verify outcome
            paymentMethodActor.Tell("state", TestActor);
            var actual = ExpectMsg<Domain.Entities.PaymentMethod>();
            actual.Id.Should().Be(id);
            actual.Name.Should().Be(createPaymentMethodCommand.Name);
        }

        [Theory(DisplayName = "Payment method actor should recover successfully (snapshot)")]
        [DefaultData]
        public void PaymentMethodActorShouldRecoverSuccessfullySnapshot(
            Domain.Entities.PaymentMethod paymentMethod)
        {
            // Setup
            var probe = CreateTestProbe();
            PersistenceHelper.InitializeSnapshot(probe, paymentMethod.Id.ToString(), paymentMethod);

            // Exercise
            var paymentMethodActor = Sys.ActorOf(PaymentMethodActor.GetProps(paymentMethod.Id));

            // Verify outcome
            paymentMethodActor.Tell("state", TestActor);
            var actual = ExpectMsg<Domain.Entities.PaymentMethod>();
            actual.Id.Should().Be(paymentMethod.Id);
            actual.Name.Should().Be(paymentMethod.Name);
        }
    }
}