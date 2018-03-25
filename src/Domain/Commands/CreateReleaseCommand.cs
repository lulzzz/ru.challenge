using MediatR;
using System;

namespace RU.Challenge.Domain.Commands
{
    public class CreateReleaseCommand : IRequest
    {
        public Guid ReleaseId { get; set; }

        public string Title { get; set; }

        public Guid ArtistId { get; set; }

        public Guid GenreId { get; set; }

        public string CoverArtUrl { get; set; }

        public Guid UserId { get; set; }

        public CreateReleaseCommand(string title, Guid artistId, Guid genreId, Guid releaseId, Guid userId, string coverArtUrl)
        {
            Title = title;
            ArtistId = artistId;
            GenreId = genreId;
            ReleaseId = releaseId;
            UserId = userId;
            CoverArtUrl = coverArtUrl;
        }
    }
}