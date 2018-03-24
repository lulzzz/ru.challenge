using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using RU.Challenge.Domain.Commands;
using RU.Challenge.Domain.Entities;
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
        public async Task<IEnumerable<Artist>> GetArtists()
        {
            return await _mediator.Send(new GetArtistsByNameQuery(name: null));
        }

        [HttpGet]
        [Route("artists/name/{name}")]
        public async Task<IEnumerable<Artist>> GetArtistsByName([FromRoute] string name)
        {
            return await _mediator.Send(new GetArtistsByNameQuery(name));
        }

        [HttpGet]
        [Route("artists/id/{id}")]
        public async Task<Artist> GetArtistById([FromRoute] Guid id)
        {
            return await _mediator.Send(new GetArtistByIdQuery(id));
        }

        [HttpPost]
        [Route("artists")]
        public async Task<IActionResult> AddArtist([FromBody] CreateArtistCommand command)
        {
            var artistId = Guid.NewGuid();
            command.SetId(artistId);
            await _mediator.Send(command);
            return Created(new Uri($"{Request.Host}{Request.Path}/id/{artistId}"), artistId);
        }
    }
}