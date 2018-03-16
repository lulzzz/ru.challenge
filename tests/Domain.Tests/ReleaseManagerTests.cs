using FluentAssertions;
using RU.Challenge.Domain.Entities;
using RU.Challenge.Fixtures.Attributes;
using System;
using System.Linq;
using Xunit;

namespace RU.Challenge.Domain
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