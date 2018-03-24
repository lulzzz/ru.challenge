using System;

namespace RU.Challenge.Infrastructure.Akka.States
{
    public class TrackState
    {
        public Guid Id { get; private set; }

        public Guid ReleaseId { get; private set; }

        public string Name { get; private set; }

        public string SongUrl { get; private set; }

        public Guid GenreId { get; private set; }

        public Guid ArtistId { get; private set; }

        public int Order { get; private set; }

        public TrackState(
            Guid id,
            Guid releaseId,
            string name, 
            string songUrl,
            Guid genreId,
            Guid artistId,
            int order)
        {
            Id = id;
            ReleaseId = releaseId;
            Name = name;
            SongUrl = songUrl;
            GenreId = genreId;
            ArtistId = artistId;
            Order = order;
        }
    }
}