using FluentAssertions;
using RU.Challenge.Fixtures.Attributes;
using System;
using Xunit;

namespace RU.Challenge.Domain.Entities
{
    public class SubscriptionTests
    {
        [Theory(DisplayName = "Subscription should be created with expected params")]
        [DefaultData]
        public void SubscriptionShouldBeCreatedWithExpectedParams(
            Guid id,
            DateTimeOffset expirationDate,
            decimal amount)
        {
            // Exercise
            var actual = Subscription.Create(id, expirationDate, amount);

            // Verify outcome
            actual.Id.Should().Be(id);
            actual.ExpirationDate.Should().Be(expirationDate);
            actual.Amount.Should().Be(amount);
        }

        [Theory(DisplayName = "Subscription should contain added payment method")]
        [DefaultData]
        public void SubscriptionShouldContainAddedPaymentMethod(
            Subscription subscription, PaymentMethod paymentMethod)
        {
            // Exercise
            subscription.SetPaymentMethod(paymentMethod);

            // Verify outcome
            subscription.PaymentMethod.Should().Be(paymentMethod);
        }

        [Theory(DisplayName = "Subscription should contain added distribution platforms")]
        [DefaultData]
        public void SubscriptionShouldContainAddedDistributionPlatforms(
            Subscription subscription, DistributionPlatform dp1, DistributionPlatform dp2)
        {
            // Pre condition
            subscription.DistributionPlatforms.Should().BeEmpty();

            // Exercise
            subscription.AddDistributionPlatform(dp1);
            subscription.AddDistributionPlatform(dp2);

            // Verify outcome
            subscription.DistributionPlatforms.Should().ContainInOrder(dp1, dp2);
        }
    }
}