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
    public class GenreController : Controller
    {
        private readonly IMediator _mediator;

        public GenreController(IMediator mediator)
            => _mediator = mediator;

        [HttpGet]
        [Route("genres")]
        public async Task<IEnumerable<Domain.Entities.Genre>> GetGenres()
        {
            return await _mediator.Send(new GetAllGenresQuery());
        }

        [HttpPost]
        [Route("genres")]
        public async Task<IActionResult> AddGenre([FromBody] Domain.Commands.CreateGenreCommand command)
        {
            var genreId = Guid.NewGuid();
            command.GenreId = genreId;
            await _mediator.Send(command);
            return Accepted(genreId);
        }
    }
}