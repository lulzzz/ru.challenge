using RU.Challenge.Domain.Enums;
using System;
using System.Collections.Generic;

namespace RU.Challenge.Infrastructure.Dapper.DTO
{
    internal class ReleaseQueryResult
    {
        internal Guid Id { get; set; }

        internal string Title { get; set; }

        internal string CoverArtUrl { get; set; }

        internal ReleaseStatus Status { get; set; }

        internal Guid GenreId { get; set; }

        internal Guid ArtistId { get; set; }

        internal Guid? SubscriptionId { get; set; }

        internal IEnumerable<Guid> TracksIds { get; set; }
    }
}