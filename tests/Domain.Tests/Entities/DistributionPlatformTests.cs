using FluentAssertions;
using RU.Challenge.Fixtures.Attributes;
using System;
using Xunit;

namespace RU.Challenge.Domain.Entities
{
    public class DistributionPlatformTests
    {
        [Theory(DisplayName = "Distribution platform should be created with expected params")]
        [DefaultData]
        public void DistributionPlatformShouldBeCreatedWithExpectedParams(Guid id, string name)
        {
            // Exercise
            var actual = DistributionPlatform.Create(id, name);

            // Verify outcome
            actual.Id.Should().Be(id);
            actual.Name.Should().Be(name);
        }

        [Theory(DisplayName = "Clone distribution platform should generate other distribution platform instance")]
        [DefaultData]
        public void CloneDistributionPlatformShouldGenerateOtherDistributionPlatformInstance(
            DistributionPlatform distributionPlatform)
        {
            // Exercise
            var actual = distributionPlatform.Clone();

            // Verify outcome
            actual.Should().NotBe(distributionPlatform);
            actual.Should().BeEquivalentTo(distributionPlatform);
        }
    }
}