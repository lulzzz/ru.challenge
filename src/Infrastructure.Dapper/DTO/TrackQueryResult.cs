using System;

namespace RU.Challenge.Infrastructure.Dapper.DTO
{
    internal class TrackQueryResult
    {
        internal Guid Id { get; set; }

        internal string Name { get; set; }

        internal Guid ArtistId { get; set; }

        internal Guid GenreId { get; set; }

        internal int Order { get; set; }

        internal string SongUrl { get; set; }
    }
}