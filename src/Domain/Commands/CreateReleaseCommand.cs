using MediatR;
using RU.Challenge.Domain.Entities;
using System;

namespace RU.Challenge.Domain.Commands
{
    public class CreateReleaseCommand : IRequest
    {
        public Guid ReleaseId { get; private set; }

        public string Title { get; set; }

        public Guid ArtistId { get; set; }

        public Guid GenreId { get; set; }

        public string CoverArtUrl { get; set; }

        public Guid UserId { get; private set; }

        public CreateReleaseCommand(string title, Guid artistId, Guid genreId, string coverArtUrl)
        {
            Title = title;
            ArtistId = artistId;
            GenreId = genreId;
            CoverArtUrl = coverArtUrl;
        }

        public CreateReleaseCommand(Guid releaseId, string title, Guid artistId, Guid genreId, string coverArtUrl, Guid userId)
            : this(title, artistId, genreId, coverArtUrl)
        {
            ReleaseId = releaseId;
            UserId = userId;
        }
    }
}