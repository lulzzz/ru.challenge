using FluentAssertions;
using RU.Challenge.Fixtures.Attributes;
using System;
using Xunit;

namespace RU.Challenge.Domain.Entities
{
    public class GenreTests
    {
        [Theory(DisplayName = "Genre should be created with expected params")]
        [DefaultData]
        public void GenreShouldBeCreatedWithExpectedParams(Guid id, string name)
        {
            // Exercise
            var actual = Genre.Create(id, name);

            // Verify outcome
            actual.Id.Should().Be(id);
            actual.Name.Should().Be(name);
        }

        [Theory(DisplayName = "Clone genre should generate other genre instance")]
        [DefaultData]
        public void CloneGenreShouldGenerateOtherGenreInstance(Genre genre)
        {
            // Exercise
            var actual = genre.Clone();

            // Verify outcome
            actual.Should().NotBe(genre);
            actual.Should().BeEquivalentTo(genre);
        }
    }
}