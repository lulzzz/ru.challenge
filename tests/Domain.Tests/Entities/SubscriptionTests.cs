using FluentAssertions;
using RU.Challenge.Fixtures.Attributes;
using System;
using System.Collections.Immutable;
using Xunit;

namespace RU.Challenge.Domain.Entities
{
    public class SubscriptionTests
    {
        [Theory(DisplayName = "Subscription should be created with expected params")]
        [DefaultData]
        public void SubscriptionShouldBeCreatedWithExpectedParams(
            Guid id,
            PaymentMethod paymentMethod,
            IImmutableList<DistributionPlatform> distributionPlatforms,
            DateTimeOffset expirationDate,
            decimal amount)
        {
            // Exercise
            var actual = Subscription.Create(id, paymentMethod, distributionPlatforms, expirationDate, amount);

            // Verify outcome
            actual.Id.Should().Be(id);
            actual.PaymentMethod.Should().Be(paymentMethod);
            actual.DistributionPlatforms.Should().AllBeEquivalentTo(distributionPlatforms);
            actual.ExpirationDate.Should().Be(expirationDate);
            actual.Amount.Should().Be(amount);
        }

        [Theory(DisplayName = "Clone subscription should generate other subscription instance")]
        [DefaultData]
        public void CloneSubscriptionShouldGenerateOtherSubscriptionInstance(Subscription subscription)
        {
            // Exercise
            var actual = subscription.Clone();

            // Verify outcome
            actual.Should().NotBe(subscription);
            actual.Should().BeEquivalentTo(subscription);
        }
    }
}