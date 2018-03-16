using System;

namespace RU.Challenge.Domain.Entities
{
    public class Track
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public Artist Artist { get; private set; }

        public Genre Genre { get; private set; }

        public int? Order { get; private set; }

        public string SongUrl { get; private set; }

        public void SetOrder(int order)
            => Order = order;

        private Track(Guid id, string name, string songUrl, Genre genre, Artist artist) : this()
        {
            Id = id;
            Name = name;
            SongUrl = songUrl;
            Genre = genre;
            Artist = artist;
        }

        private Track()
        {
        }

        public static Track Create(string name, string songUrl, Genre genre, Artist artist)
            => new Track(Guid.NewGuid(), name, songUrl, genre, artist);
    }
}