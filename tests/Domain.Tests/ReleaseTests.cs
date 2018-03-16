using AutoFixture;
using FluentAssertions;
using RU.Challenge.Domain.Entities;
using RU.Challenge.Domain.Exceptions;
using RU.Challenge.Fixtures.Attributes;
using System;
using System.Linq;
using Xunit;

namespace RU.Challenge.Domain
{
    public class ReleaseTests
    {
        public class ActiveSubcriptionCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.CustomizeCtorParameter<Subscription>(
                    paramName: "expirationDate", value: DateTimeOffset.Now.AddYears(1));
            }
        }
        public class ExpiredSubcriptionCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.CustomizeCtorParameter<Subscription>(
                    paramName: "expirationDate", value: DateTimeOffset.Now.AddYears(-1));
            }
        }



        [Theory(DisplayName = "Release should be created with expected params")]
        [DefaultData]
        public void ReleaseShouldBeCreatedWithExpectedParams(
            string title,
            Artist artist,
            Genre genre,
            string coverArtUrl)
        {
            // Exercise
            var actual = Release.Create(title, artist, genre, coverArtUrl);

            // Verify outcome
            actual.Title.Should().Be(title);
            actual.Artist.Should().Be(artist);
            actual.Genre.Should().Be(genre);
            actual.CoverArtUrl.Should().Be(coverArtUrl);
            actual.Id.Should().NotBeEmpty();
            actual.Tracks.Should().BeEmpty();
        }

        [Theory(DisplayName = "Release should contain added track with sequential order")]
        [InlineDefaultData]
        [InlineDefaultData(typeof(ExpiredSubcriptionCustomization))]
        public void ReleaseShouldContainAddedTrackWithSequentialOrder(
            Release release,
            Track track1,
            Track track2)
        {
            // Pre condition
            release.Tracks.Should().BeEmpty();

            // Exercise
            release.AddTrack(track1);
            release.AddTrack(track2);

            // Verify outcome
            release.Tracks.Should().ContainInOrder(track1, track2);
            release.Tracks.First().Order.Should().Be(1);
            release.Tracks.Skip(count: 1).First().Order.Should().Be(2);
        }

        [Theory(DisplayName = "Release should throw domain exception when adding a track and subscription is not null and active")]
        [DefaultData(typeof(ActiveSubcriptionCustomization))]
        public void ReleaseShouldThrowDomainExceptionWhenAddingTrackAndSubscriptionIsNotNullAndActive(
            Release release,
            Subscription subscription,
            Track track)
        {
            // Setup
            release.AssociateSubscription(subscription);

            // Exercise
            Action act = () => release.AddTrack(track);

            // Verify outcome
            act.Should().ThrowExactly<DomainException>();
        }

        [Theory(DisplayName = "Release should contain added subscription")]
        [InlineDefaultData]
        [InlineDefaultData(typeof(ExpiredSubcriptionCustomization))]
        public void ReleaseShouldContainAddedSubscription(
            Release release, Subscription subscription)
        {
            // Pre condition
            release.Subscription.Should().BeNull();

            // Exercise
            release.AssociateSubscription(subscription);

            // Verify outcome
            release.Subscription.Should().Be(subscription);
        }

        [Theory(DisplayName = "Release should throw domain exception when adding a subscription and other subscription is active")]
        [DefaultData(typeof(ActiveSubcriptionCustomization))]
        public void ReleaseShouldThrowDomainExceptionWhenAddingASubscriptionAndOtherSubscriptionIsActive(
            Release release, Subscription subscription)
        {
            // Setup
            release.AssociateSubscription(subscription);

            // Exercise
            Action act = () => release.AssociateSubscription(subscription);

            // Verify outcome
            act.Should().ThrowExactly<DomainException>();
        }
    }
}