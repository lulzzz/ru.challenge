using RU.Challenge.Domain.Commands;
using System;

namespace RU.Challenge.Domain.Events
{
    public class CreateTrackEvent
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public string SongUrl { get; private set; }

        public Guid GenreId { get; private set; }

        public Guid ArtistId { get; private set; }

        public CreateTrackEvent(Guid id, string name, string songUrl, Guid genreId, Guid artistId)
        {
            Id = id;
            Name = name;
            SongUrl = songUrl;
            GenreId = genreId;
            ArtistId = artistId;
        }

        public static CreateTrackEvent CreateFromCommand(CreateTrackCommand command, Guid id)
            => new CreateTrackEvent(id, command.Name, command.SongUrl, command.GenreId, command.ArtistId);
    }
}