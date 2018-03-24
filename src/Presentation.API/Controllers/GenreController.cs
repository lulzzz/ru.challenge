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
    public class GenreController : Controller
    {
        private readonly IMediator _mediator;

        public GenreController(IMediator mediator)
            => _mediator = mediator;

        [HttpGet]
        [Route("genres")]
        public async Task<IEnumerable<Genre>> GetGenres()
        {
            return await _mediator.Send(new GetAllGenresQuery());
        }

        [HttpGet]
        [Route("genres/id/{id}")]
        public async Task<Genre> GetGenreById([FromRoute] Guid id)
        {
            return await _mediator.Send(new GetGenreByIdQuery(id));
        }

        [HttpPost]
        [Route("genres")]
        public async Task<IActionResult> AddGenre([FromBody] CreateGenreCommand command)
        {
            var genreId = Guid.NewGuid();
            command.SetId(genreId);
            await _mediator.Send(command);
            return Created(new Uri($"{Request.Host}{Request.Path}/id/{genreId}"), genreId);
        }
    }
}