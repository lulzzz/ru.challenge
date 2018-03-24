using MediatR;
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

        public void SetId(Guid releaseId)
        {
            ReleaseId = releaseId;
        }

        public void SetUserId(Guid userId)
        {
            UserId = userId;
        }
    }
}