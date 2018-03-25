using MediatR;
using System;

namespace RU.Challenge.Domain.Commands
{
    public class CreateTrackCommand : IRequest
    {
        public Guid TrackId { get; set; }

        public Guid ReleaseId { get; set; }

        public string Name { get; set; }

        public string SongUrl { get; set; }

        public Guid GenreId { get; set; }

        public Guid ArtistId { get; set; }

        public int Order { get; private set; }

        public CreateTrackCommand(string name, string songUrl, Guid genreId, Guid artistId, Guid releaseId, Guid trackId)
        {
            Name = name;
            SongUrl = songUrl;
            GenreId = genreId;
            ArtistId = artistId;
            ReleaseId = releaseId;
            TrackId = trackId;
        }

        public void SetOrder(int order)
            => Order = order;
    }
}