using FluentAssertions;
using RU.Challenge.Fixtures.Attributes;
using System;
using Xunit;

namespace RU.Challenge.Domain.Entities
{
    public class PaymentMethodTests
    {
        [Theory(DisplayName = "Payment method should be created with expected params")]
        [DefaultData]
        public void PaymentMethodShouldBeCreatedWithExpectedParams(Guid id, string name)
        {
            // Exercise
            var actual = PaymentMethod.Create(id, name);

            // Verify outcome
            actual.Id.Should().Be(id);
            actual.Name.Should().Be(name);
        }

        [Theory(DisplayName = "Clone payment method should generate other payment method instance")]
        [DefaultData]
        public void ClonePaymentMethodShouldGenerateOtherPaymentMethodInstance(PaymentMethod paymentMethod)
        {
            // Exercise
            var actual = paymentMethod.Clone();

            // Verify outcome
            actual.Should().NotBe(paymentMethod);
            actual.Should().BeEquivalentTo(paymentMethod);
        }
    }
}