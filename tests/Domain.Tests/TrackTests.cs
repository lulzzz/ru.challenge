using FluentAssertions;
using RU.Challenge.Domain.Entities;
using RU.Challenge.Fixtures.Attributes;
using Xunit;

namespace RU.Challenge.Domain
{
    public class TrackTests
    {
        [Theory(DisplayName = "Track should be created with expected params")]
        [DefaultData]
        public void TrackShouldBeCreatedWithExpectedParams(
            string name,
            string songUrl,
            Genre genre,
            Artist artist)
        {
            // Exercise
            var actual = Track.Create(name, songUrl, genre, artist);

            // Verify outcome
            actual.Name.Should().Be(name);
            actual.SongUrl.Should().Be(songUrl);
            actual.Genre.Should().Be(genre);
            actual.Artist.Should().Be(artist);
            actual.Id.Should().NotBeEmpty();
            actual.Order.Should().Be(default(int?));
        }

        [Theory(DisplayName = "Track order should be correctly assigned")]
        [DefaultData]
        public void TrackOrderShouldBeCorrectlyAssigned(Track track, int order)
        {
            // Pre condition
            track.Order.Should().Be(default(int?));

            // Exercise
            track.SetOrder(order);

            // Verify outcome
            track.Order.Should().Be(order);
        }
    }
}