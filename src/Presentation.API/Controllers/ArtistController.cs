using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using RU.Challenge.Domain.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RU.Challenge.Presentation.API.Controllers
{
    [Route("api")]
    [Authorize(Roles = "api_access, api_release_manager")]
    public class ArtistController : Controller
    {
        private readonly IMediator _mediator;

        public ArtistController(IMediator mediator)
            => _mediator = mediator;

        [HttpGet]
        [Route("artist")]
        public async Task<IEnumerable<Domain.Entities.Artist>> GetArtists()
        {
            return await _mediator.Send(new GetArtistsByNameQuery(name: null));
        }

        [HttpGet]
        [Route("artist/{name}")]
        public async Task<IEnumerable<Domain.Entities.Artist>> GetArtistsByName([FromRoute] string name)
        {
            return await _mediator.Send(new GetArtistsByNameQuery(name));
        }

        [HttpPost]
        [Route("artist")]
        public async Task<IActionResult> AddArtist([FromBody] Domain.Commands.CreateArtistCommand command)
        {
            await _mediator.Send(command);
            return Accepted();
        }
    }
}