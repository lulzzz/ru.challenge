using MediatR;
using System.Collections.Generic;

namespace RU.Challenge.Domain.Queries
{
    public class GetArtistsByNameQuery : IRequest<IEnumerable<Entities.Artist>>
    {
        public string Name { get; set; }

        public GetArtistsByNameQuery(string name)
            => Name = name;
    }
}