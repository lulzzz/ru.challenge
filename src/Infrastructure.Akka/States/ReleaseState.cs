using RU.Challenge.Domain.Enums;
using System;
using System.Collections.Generic;

namespace RU.Challenge.Infrastructure.Akka.States
{
    public class ReleaseState
    {
        public Guid Id { get; private set; }

        public string Title { get; private set; }

        public Guid ArtistId { get; private set; }

        public Guid GenreId { get; private set; }

        public string CoverArtUrl { get; private set; }

        public IList<Guid> Tracks { get; private set; }

        public Guid SubscriptionId { get; internal set; }

        public Guid UserId { get; private set; }

        public ReleaseStatus Status { get; internal set; }

        public ReleaseState(
            Guid id,
            string title,
            Guid artistId,
            Guid genreId,
            string coverArtUrl,
            IList<Guid> tracks,
            Guid subscriptionId,
            Guid userId,
            ReleaseStatus status)
        {
            Id = id;
            Title = title;
            ArtistId = artistId;
            GenreId = genreId;
            CoverArtUrl = coverArtUrl;
            Tracks = tracks;
            SubscriptionId = subscriptionId;
            UserId = userId;
            Status = status;
        }
    }
}