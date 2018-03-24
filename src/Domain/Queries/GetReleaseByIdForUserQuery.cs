using MediatR;
using System;

namespace RU.Challenge.Domain.Queries
{
    public class GetReleaseByIdForUserQuery : IRequest<Entities.Release>
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public GetReleaseByIdForUserQuery(Guid id, Guid userId)
        {
            Id = id;
            UserId = userId;
        }
    }
}