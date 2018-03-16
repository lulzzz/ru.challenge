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
        public void TrackShouldBeCreatedWithExpectedParams(Guid id, string name, string songUrl)
        {
            // Exercise
            var actual = Track.Create(id, name, songUrl);

            // Verify outcome
            actual.Id.Should().Be(id);
            actual.Name.Should().Be(name);
            actual.SongUrl.Should().Be(songUrl);
            actual.Genre.Should().BeNull();
            actual.Artist.Should().BeNull();
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

        [Theory(DisplayName = "Track artist should be correctly assigned")]
        [DefaultData]
        public void TrackArtistShouldBeCorrectlyAssigned(Track track, Artist artist)
        {
            // Pre condition
            track.Artist.Should().BeNull();

            // Exercise
            track.SetArtist(artist);

            // Verify outcome
            track.Artist.Should().Be(artist);
        }

        [Theory(DisplayName = "Track genre should be correctly assigned")]
        [DefaultData]
        public void TrackGenreShouldBeCorrectlyAssigned(Track track, Genre genre)
        {
            // Pre condition
            track.Genre.Should().BeNull();

            // Exercise
            track.SetGenre(genre);

            // Verify outcome
            track.Genre.Should().Be(genre);
        }
    }
}