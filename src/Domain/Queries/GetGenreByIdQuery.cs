using MediatR;
using System;

namespace RU.Challenge.Domain.Queries
{
    public class GetGenreByIdQuery : IRequest<Entities.Genre>
    {
        public Guid Id { get; set; }

        public GetGenreByIdQuery(Guid id)
            => Id = id;
    }
}