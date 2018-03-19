using MediatR;
using System.Collections.Generic;

namespace RU.Challenge.Domain.Queries
{
    public class GetAllGenresQuery : IRequest<IEnumerable<Entities.Genre>>
    {
    }
}