using FluentAssertions;
using RU.Challenge.Domain.Entities;
using RU.Challenge.Fixtures.Attributes;
using System;
using System.Collections.Immutable;
using Xunit;

namespace RU.Challenge.Domain
{
    public class SubscriptionTests
    {
        [Theory(DisplayName = "Subscription should be created with expected params")]
        [DefaultData]
        public void SubscriptionShouldBeCreatedWithExpectedParams(
            PaymentMethod paymentMethod,
            IImmutableList<DistributionPlatform> distributionPlatforms,
            DateTimeOffset expirationDate,
            decimal amount)
        {
            // Exercise
            var actual = Subscription.Create(paymentMethod, distributionPlatforms, expirationDate, amount);

            // Verify outcome
            actual.PaymentMethod.Should().Be(paymentMethod);
            actual.DistributionPlatforms.Should().AllBeEquivalentTo(distributionPlatforms);
            actual.ExpirationDate.Should().Be(expirationDate);
            actual.Amount.Should().Be(amount);
            actual.Id.Should().NotBeEmpty();
        }
    }
}