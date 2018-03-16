using System;

namespace RU.Challenge.Infrastructure.Akka.Snapshot
{
    public class TrackAggregateSnapshot
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public Guid ArtistId { get; private set; }

        public Guid GenreId { get; private set; }

        public int? Order { get; private set; }

        public string SongUrl { get; private set; }

        public TrackAggregateSnapshot(
            Guid id,
            string name,
            Guid artistId,
            Guid genreId,
            int? order,
            string songUrl)
        {
            Id = id;
            Name = name;
            ArtistId = artistId;
            GenreId = genreId;
            Order = order;
            SongUrl = songUrl;
        }
    }
}