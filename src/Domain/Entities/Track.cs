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

        public void SetOrder(int? order)
            => Order = order;

        public void SetArtist(Artist artist)
            => Artist = artist;

        public void SetGenre(Genre genre)
            => Genre = genre;

        private Track(Guid id, string name, string songUrl)
        {
            Id = id;
            Name = name;
            SongUrl = songUrl;
        }

        public static Track Create(Guid id, string name, string songUrl)
            => new Track(id, name, songUrl);
    }
}