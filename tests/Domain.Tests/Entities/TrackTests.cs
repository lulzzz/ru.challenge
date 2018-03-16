using FluentAssertions;
using RU.Challenge.Fixtures.Attributes;
using System;
using Xunit;

namespace RU.Challenge.Domain.Entities
{
    public class TrackTests
    {
        [Theory(DisplayName = "Track should be created with expected params")]
        [DefaultData]
        public void TrackShouldBeCreatedWithExpectedParams(
            Guid id,
            string name,
            string songUrl,
            Genre genre,
            Artist artist)
        {
            // Exercise
            var actual = Track.Create(id, name, songUrl, genre, artist);

            // Verify outcome
            actual.Id.Should().Be(id);
            actual.Name.Should().Be(name);
            actual.SongUrl.Should().Be(songUrl);
            actual.Genre.Should().Be(genre);
            actual.Artist.Should().Be(artist);
            actual.Order.Should().Be(default(int?));
        }

        [Theory(DisplayName = "Clone track should generate other track instance")]
        [DefaultData]
        public void CloneTrackShouldGenerateOtherTrackInstance(Track track)
        {
            // Exercise
            var actual = track.Clone();

            // Verify outcome
            actual.Should().NotBe(track);
            actual.Should().BeEquivalentTo(track);
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