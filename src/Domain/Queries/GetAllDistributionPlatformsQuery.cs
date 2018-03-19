using MediatR;
using System.Collections.Generic;

namespace RU.Challenge.Domain.Queries
{
    public class GetAllDistributionPlatformsQuery : IRequest<IEnumerable<Entities.DistributionPlatform>>
    {
    }
}