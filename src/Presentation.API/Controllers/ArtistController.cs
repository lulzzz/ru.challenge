using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using RU.Challenge.Domain.Queries;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RU.Challenge.Presentation.API.Controllers
{
    [Route("api")]
    [Authorize(Roles = "DataEntry, ReleaseManager")]
    public class ArtistController : Controller
    {
        private readonly IMediator _mediator;

        public ArtistController(IMediator mediator)
            => _mediator = mediator;

        [HttpGet]
        [Route("artists")]
        public async Task<IEnumerable<Domain.Entities.Artist>> GetArtists()
        {
            return await _mediator.Send(new GetArtistsByNameQuery(name: null));
        }

        [HttpGet]
        [Route("artists/{name}")]
        public async Task<IEnumerable<Domain.Entities.Artist>> GetArtistsByName([FromRoute] string name)
        {
            return await _mediator.Send(new GetArtistsByNameQuery(name));
        }

        [HttpPost]
        [Route("artists")]
        public async Task<IActionResult> AddArtist([FromBody] Domain.Commands.CreateArtistCommand command)
        {
            var artistId = Guid.NewGuid();
            command.ArtistId = artistId;
            await _mediator.Send(command);
            return Accepted(artistId);
        }
    }
}