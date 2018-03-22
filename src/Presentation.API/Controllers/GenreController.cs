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
    public class GenreController : Controller
    {
        private readonly IMediator _mediator;

        public GenreController(IMediator mediator)
            => _mediator = mediator;

        [HttpGet]
        [Route("genre")]
        public async Task<IEnumerable<Domain.Entities.Genre>> GetGenres()
        {
            return await _mediator.Send(new GetAllGenresQuery());
        }

        [HttpPost]
        [Route("genre")]
        public async Task<IActionResult> AddGenre([FromBody] Domain.Commands.CreateGenreCommand command)
        {
            await _mediator.Send(command);
            return Accepted();
        }
    }
}