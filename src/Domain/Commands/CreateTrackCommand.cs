using MediatR;
using System;

namespace RU.Challenge.Domain.Commands
{
    public class CreateTrackCommand : IRequest
    {
        public string Name { get; set; }

        public string SongUrl { get; set; }

        public Guid GenreId { get; set; }

        public Guid ArtistId { get; set; }

        public CreateTrackCommand(string name, string songUrl, Guid genreId, Guid artistId)
        {
            Name = name;
            SongUrl = songUrl;
            GenreId = genreId;
            ArtistId = artistId;
        }
    }
}