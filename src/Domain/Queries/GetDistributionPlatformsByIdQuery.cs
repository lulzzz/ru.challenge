using MediatR;
using System;
using System.Collections.Generic;

namespace RU.Challenge.Domain.Queries
{
    public class GetDistributionPlatformsByIdQuery : IRequest<IEnumerable<Entities.DistributionPlatform>>
    {
        public IEnumerable<Guid> Ids { get; set; }

        public GetDistributionPlatformsByIdQuery(IEnumerable<Guid> ids)
            => Ids = ids;
    }
}