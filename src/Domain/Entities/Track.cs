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
        private Track(Track other) : this()
        {
            Id = other.Id;
            Name = other.Name;
            SongUrl = other.SongUrl;
            Genre = other.Genre;
            Artist = other.Artist;
            Order = other.Order;
        }

        private Track()
        {
        }

        public Track Clone() => new Track(this);

        public static Track Create(Guid id, string name, string songUrl, Genre genre, Artist artist)
            => new Track(id, name, songUrl, genre, artist);
    }
}