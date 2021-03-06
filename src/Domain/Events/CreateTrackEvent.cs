﻿using RU.Challenge.Domain.Commands;
using System;

namespace RU.Challenge.Domain.Events
{
    public class CreateTrackEvent
    {
        public Guid Id { get; private set; }

        public Guid ReleaseId { get; private set; }

        public string Name { get; private set; }

        public string SongUrl { get; private set; }

        public Guid GenreId { get; private set; }

        public Guid ArtistId { get; private set; }

        public int Order { get; private set; }

        public CreateTrackEvent(Guid id, Guid releaseId, string name, string songUrl, Guid genreId, Guid artistId, int order)
        {
            Id = id;
            ReleaseId = releaseId;
            Name = name;
            SongUrl = songUrl;
            GenreId = genreId;
            ArtistId = artistId;
            Order = order;
        }

        public static CreateTrackEvent CreateFromCommand(CreateTrackCommand command)
            => new CreateTrackEvent(command.TrackId, command.ReleaseId, command.Name, command.SongUrl, command.GenreId, command.ArtistId, command.Order);
    }
}