using MediatR;
using System;

namespace RU.Challenge.Domain.Commands
{
    public class CreateTrackCommand : IRequest
    {
        public Guid TrackId { get; private set; }

        public Guid ReleaseId { get; private set; }

        public string Name { get; set; }

        public string SongUrl { get; set; }

        public Guid GenreId { get; set; }

        public Guid ArtistId { get; set; }

        public int Order { get; private set; }

        public CreateTrackCommand(string name, string songUrl, Guid genreId, Guid artistId)
        {
            Name = name;
            SongUrl = songUrl;
            GenreId = genreId;
            ArtistId = artistId;
        }

        public void SetReleaseId(Guid releaseId)
            => ReleaseId = releaseId;

        public void SetTrackId(Guid trackId)
            => TrackId = trackId;

        public void SetOrder(int order)
            => Order = order;
    }
}