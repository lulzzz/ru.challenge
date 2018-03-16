using FluentAssertions;
using RU.Challenge.Domain.Entities;
using RU.Challenge.Fixtures.Attributes;
using Xunit;

namespace RU.Challenge.Domain
{
    public class GenreTests
    {
        [Theory(DisplayName = "Genre should be created with expected params")]
        [DefaultData]
        public void GenreShouldBeCreatedWithExpectedParams(string name)
        {
            // Exercise
            var actual = Genre.Create(name);

            // Verify outcome
            actual.Name.Should().Be(name);
            actual.Id.Should().NotBeEmpty();
        }
    }
}