using FluentAssertions;
using RU.Challenge.Domain.Entities;
using RU.Challenge.Fixtures.Attributes;
using Xunit;

namespace RU.Challenge.Domain
{
    public class DistributionPlatformTests
    {
        [Theory(DisplayName = "Distribution platform should be created with expected params")]
        [DefaultData]
        public void DistributionPlatformShouldBeCreatedWithExpectedParams(string name)
        {
            // Exercise
            var actual = DistributionPlatform.Create(name);

            // Verify outcome
            actual.Name.Should().Be(name);
            actual.Id.Should().NotBeEmpty();
        }
    }
}