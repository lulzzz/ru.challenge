using RU.Challenge.Domain.Commands;
using System;

namespace RU.Challenge.Domain.Events
{
    public class CreateReleaseEvent
    {
        public Guid Id { get; private set; }

        public string Title { get; private set; }

        public Guid ArtistId { get; private set; }

        public Guid GenreId { get; private set; }

        public string CoverArtUrl { get; private set; }

        public Guid UserId { get; private set; }

        public CreateReleaseEvent(
            Guid id,
            string title,
            Guid artistId,
            Guid genreId,
            string coverArtUrl,
            Guid userId)
        {
            Id = id;
            Title = title;
            ArtistId = artistId;
            GenreId = genreId;
            CoverArtUrl = coverArtUrl;
            UserId = userId;
        }

        public static CreateReleaseEvent CreateFromCommand(CreateReleaseCommand command)
            => new CreateReleaseEvent(command.ReleaseId, command.Title, command.ArtistId, command.GenreId, command.CoverArtUrl, command.UserId);
    }
}