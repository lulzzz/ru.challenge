using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RU.Challenge.Domain.Commands;
using RU.Challenge.Domain.Entities;
using RU.Challenge.Domain.Queries;
using RU.Challenge.Infrastructure.Identity.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RU.Challenge.Presentation.API.Controllers
{
    [Route("api")]
    [Authorize(Roles = "ReleaseManager")]
    public class ReleaseController : Controller
    {
        private readonly IMediator _mediator;

        public ReleaseController(IMediator mediator)
            => _mediator = mediator;

        [HttpGet]
        [Route("releases")]
        public async Task<IEnumerable<Release>> GetReleases()
        {
            return await _mediator.Send(new GetAllReleasesForUserQuery(User.Claims.GetUserId()));
        }

        [HttpGet]
        [Route("releases/id/{id}")]
        public async Task<Release> GetReleseById([FromRoute] Guid id)
        {
            return await _mediator.Send(new GetReleaseByIdForUserQuery(id, User.Claims.GetUserId()));
        }

        [HttpPost]
        [Route("releases")]
        public async Task<IActionResult> AddRelease([FromBody] CreateReleaseCommand command)
        {
            var artist = await _mediator.Send(new GetArtistByIdQuery(command.ArtistId));

            if (artist == null)
                return BadRequest($"The artist: {command.ArtistId} does not exist");

            var genre = await _mediator.Send(new GetGenreByIdQuery(command.GenreId));

            if (genre == null)
                return BadRequest($"The genre: {command.GenreId} does not exist");

            var releaseId = Guid.NewGuid();
            command.SetId(releaseId);
            command.SetUserId(User.Claims.GetUserId());
            await _mediator.Send(command);
            return Created(new Uri($"{Request.Host}{Request.Path}/id/{releaseId}"), releaseId);
        }
    }
}