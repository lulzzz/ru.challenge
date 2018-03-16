using System;

namespace RU.Challenge.Domain.Commands
{
    public class CreateTrackCommand
    {
        public string Name { get; private set; }

        public string SongUrl { get; private set; }

        public Guid GenreId { get; private set; }

        public Guid ArtistId { get; private set; }

        public CreateTrackCommand(string name, string songUrl, Guid genreId, Guid artistId)
        {
            Name = name;
            SongUrl = songUrl;
            GenreId = genreId;
            ArtistId = artistId;
        }
    }
}