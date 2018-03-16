using FluentAssertions;
using RU.Challenge.Fixtures.Attributes;
using System;
using Xunit;

namespace RU.Challenge.Domain.Entities
{
    public class ArtistTests
    {
        [Theory(DisplayName = "Artist should be created with expected params")]
        [DefaultData]
        public void ArtistShouldBeCreatedWithExpectedParams(Guid id, int age, string name)
        {
            // Exercise
            var actual = Artist.Create(id, age, name);

            // Verify outcome
            actual.Id.Should().Be(id);
            actual.Age.Should().Be(age);
            actual.Name.Should().Be(name);
        }

        [Theory(DisplayName = "Clone artist should generate other artist instance")]
        [DefaultData]
        public void CloneArtistShouldGenerateOtherArtistInstance(Artist artist)
        {
            // Exercise
            var actual = artist.Clone();

            // Verify outcome
            actual.Should().NotBe(artist);
            actual.Should().BeEquivalentTo(artist);
        }
    }
}