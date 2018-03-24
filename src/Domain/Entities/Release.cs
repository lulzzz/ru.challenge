using RU.Challenge.Domain.Enums;
using RU.Challenge.Domain.Exceptions;
using System;
using System.Collections.Immutable;

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

        public ReleaseStatus Status { get; private set; }

        public void SetArtist(Artist artist)
            => Artist = artist;
        
        public void SetGenre(Genre genre)
            => Genre = genre;

        public void SetSubscription(Subscription subscription)
            => Subscription = subscription;

        public void AddTrack(Track track)
            => Tracks = Tracks.Add(track);

        private Release(
            Guid id,
            string title,
            string coverArtUrl,
            ReleaseStatus status) : this()
        {
            Id = id;
            Title = title;
            CoverArtUrl = coverArtUrl;
            Status = status;
        }

        private Release()
            => Tracks = ImmutableList.Create<Track>();

        public static Release Create(Guid id, string title, string coverArtUrl, ReleaseStatus status)
            => new Release(id, title, coverArtUrl, status);
    }
}