using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using RU.Challenge.Domain.Commands;
using RU.Challenge.Domain.Entities;
using RU.Challenge.Domain.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RU.Challenge.Presentation.API.Controllers
{
    [Route("api")]
    [Authorize(Roles = "DataEntry, ReleaseManager")]
    public class ArtistController : Controller
    {
        private readonly IMediator _mediator;

        public ArtistController(IMediator mediator)
            => _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        [HttpGet]
        [Route("artists")]
        public async Task<IActionResult> GetArtists()
        {
            var items = await _mediator.Send(new GetArtistsByIdQuery(ids: null));

            if (items == null || !items.Any())
                return NotFound();
            else
                return Ok(items);
        }

        [HttpGet]
        [Route("artists/name/{name}")]
        public async Task<IActionResult> GetArtistsByName([FromRoute] string name)
        {
            var items = await _mediator.Send(new GetArtistsByNameQuery(name));

            if (items == null || !items.Any())
                return NotFound();
            else
                return Ok(items);
        }

        [HttpGet]
        [Route("artists/id/{id}")]
        public async Task<IActionResult> GetArtistById([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest($@"The field(s) {string.Join(", ", ModelState
                    .Where(e => e.Value.ValidationState == ModelValidationState.Invalid).Select(e => e.Key))} are not valid");

            var item = (await _mediator.Send(new GetArtistsByIdQuery(new[] { id }))).FirstOrDefault();

            if (item == null)
                return NotFound();
            else
                return Ok(item);
        }

        [HttpPost]
        [Route("artists")]
        public async Task<IActionResult> AddArtist([FromBody] CreateArtistCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest($@"The field(s) {string.Join(", ", ModelState
                    .Where(e => e.Value.ValidationState == ModelValidationState.Invalid).Select(e => e.Key))} are not valid");

            var artistId = Guid.NewGuid();
            command.SetId(artistId);
            await _mediator.Send(command);
            return Created($"{Request.Host}{Request.Path}/id/{artistId}", artistId);
        }
    }
}