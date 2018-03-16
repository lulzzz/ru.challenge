using FluentAssertions;
using RU.Challenge.Domain.Entities;
using RU.Challenge.Fixtures.Attributes;
using Xunit;

namespace RU.Challenge.Domain
{
    public class ArtistTests
    {
        [Theory(DisplayName = "Artist should be created with expected params")]
        [DefaultData]
        public void ArtistShouldBeCreatedWithExpectedParams(int age, string name)
        {
            // Exercise
            var actual = Artist.Create(age, name);

            // Verify outcome
            actual.Age.Should().Be(age);
            actual.Name.Should().Be(name);
            actual.Id.Should().NotBeEmpty();
        }
    }
}