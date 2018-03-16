using RU.Challenge.Domain.Exceptions;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace RU.Challenge.Domain.Entities
{
    public class Release
    {
        public Guid Id { get; private set; }

        public string Title { get; private set; }

        public Artist Artist { get; private set; }

        public Genre Genre { get; private set; }

        public string CoverArtUrl { get; private set; }

        public IImmutableList<Track> Tracks { get; private set; }

        public Subscription Subscription { get; private set; }

        public void AddTrack(Track track)
        {
            if (Subscription != null && Subscription.ExpirationDate > DateTimeOffset.Now)
                throw new DomainException("Cannot add songs to a release with active subscription");

            var lastOrder = Tracks.Max(e => e.Order) ?? 0;
            track.SetOrder(lastOrder + 1);
            Tracks = Tracks.Add(track);
        }

        public void AssociateSubscription(Subscription subscription)
        {
            if (Subscription != null && Subscription.ExpirationDate > DateTimeOffset.Now)
                throw new DomainException("The release already has an active subscription");

            Subscription = subscription;
        }

        private Release(
            Guid id,
            string title,
            Artist artist,
            Genre genre,
            string coverArtUrl) : this()
        {
            Id = id;
            Title = title;
            Artist = artist;
            Genre = genre;
            CoverArtUrl = coverArtUrl;
        }

        private Release(Release other) : this()
        {
            Id = other.Id;
            Title = other.Title;
            Artist = other.Artist;
            Genre = other.Genre;
            CoverArtUrl = other.CoverArtUrl;
            Tracks = other.Tracks;
            Subscription = other.Subscription;
        }

        private Release()
            => Tracks = ImmutableList.Create<Track>();

        public Release Clone() => new Release(this);

        public static Release Create(Guid id, string title, Artist artist, Genre genre, string coverArtUrl)
            => new Release(id, title, artist, genre, coverArtUrl);
    }
}