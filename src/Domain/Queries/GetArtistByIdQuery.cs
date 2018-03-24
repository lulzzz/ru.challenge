using MediatR;
using System;

namespace RU.Challenge.Domain.Queries
{
    public class GetArtistByIdQuery : IRequest<Entities.Artist>
    {
        public Guid Id { get; set; }

        public GetArtistByIdQuery(Guid id)
            => Id = id;
    }
}