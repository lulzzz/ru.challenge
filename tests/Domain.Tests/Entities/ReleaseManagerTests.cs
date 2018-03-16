using FluentAssertions;
using RU.Challenge.Fixtures.Attributes;
using System;
using System.Linq;
using Xunit;

namespace RU.Challenge.Domain.Entities
{
    public class ReleaseManagerTests
    {
        [Theory(DisplayName = "Release manager should be created with expected params")]
        [DefaultData]
        public void ReleaseManagerShouldBeCreatedWithExpectedParams(Guid id)
        {
            // Exercise
            var actual = ReleaseManager.Create(id);

            // Verify outcome
            actual.Id.Should().Be(id);
            actual.Releases.Should().BeEmpty();
        }

        [Theory(DisplayName = "Clone release manager should generate other release manager instance")]
        [DefaultData]
        public void CloneReleaseManagerShouldGenerateOtherReleaseManagerInstance(ReleaseManager releaseManager)
        {
            // Exercise
            var actual = releaseManager.Clone();

            // Verify outcome
            actual.Should().NotBe(releaseManager);
            actual.Should().BeEquivalentTo(releaseManager);
        }

        [Theory(DisplayName = "Release manager should contain added release")]
        [DefaultData]
        public void TrackOrderShouldBeCorrectlyAssigned(
            ReleaseManager releaseManager,
            Release release)
        {
            // Pre condition
            releaseManager.Releases.Should().BeEmpty();

            // Exercise
            releaseManager.AddRelease(release);

            // Verify outcome
            releaseManager.Releases.Should().ContainSingle().And.Subject.Single().Should().Be(release);
        }
    }
}