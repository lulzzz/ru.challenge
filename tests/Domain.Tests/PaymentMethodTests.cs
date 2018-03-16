using FluentAssertions;
using RU.Challenge.Domain.Entities;
using RU.Challenge.Fixtures.Attributes;
using Xunit;

namespace RU.Challenge.Domain
{
    public class PaymentMethodTests
    {
        [Theory(DisplayName = "Payment method should be created with expected params")]
        [DefaultData]
        public void PaymentMethodShouldBeCreatedWithExpectedParams(string name)
        {
            // Exercise
            var actual = PaymentMethod.Create(name);

            // Verify outcome
            actual.Name.Should().Be(name);
            actual.Id.Should().NotBeEmpty();
        }
    }
}