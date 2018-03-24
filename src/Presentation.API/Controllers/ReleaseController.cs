using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RU.Challenge.Domain.Commands;
using RU.Challenge.Domain.Entities;
using RU.Challenge.Domain.Queries;
using RU.Challenge.Infrastructure.Identity.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
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
            return await _mediator.Send(new GetReleasesByIdForUserQuery(ids: null, userId: User.Claims.GetUserId()));
        }

        [HttpGet]
        [Route("releases/id/{id}")]
        public async Task<IActionResult> GetReleseById([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest($@"The field(s) {string.Join(", ", ModelState
                    .Where(e => e.Value.ValidationState == ModelValidationState.Invalid).Select(e => e.Key))} are not valid");

            return Ok((await _mediator.Send(new GetReleasesByIdForUserQuery(ids: new[] { id }, userId: User.Claims.GetUserId()))).FirstOrDefault());
        }

        [HttpPost]
        [Route("releases")]
        public async Task<IActionResult> AddRelease([FromBody] CreateReleaseCommand command)
        {
            var artist = await _mediator.Send(new GetArtistsByIdQuery(new[] { command.ArtistId }));

            if (artist == null)
                return BadRequest($"The artist: {command.ArtistId} does not exist");

            var genre = await _mediator.Send(new GetGenresByIdQuery(new[] { command.GenreId }));

            if (genre == null)
                return BadRequest($"The genre: {command.GenreId} does not exist");

            var releaseId = Guid.NewGuid();
            command.SetId(releaseId);
            command.SetUserId(User.Claims.GetUserId());
            await _mediator.Send(command);
            return Created(new Uri($"{Request.Host}{Request.Path}/id/{releaseId}"), releaseId);
        }

        [HttpPost]
        [Route("releases/{releaseId}/track")]
        public async Task<IActionResult> AddTrackToRelease([FromRoute] Guid releaseId, [FromBody] CreateTrackCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest($@"The field(s) {string.Join(", ", ModelState
                    .Where(e => e.Value.ValidationState == ModelValidationState.Invalid).Select(e => e.Key))} are not valid");

            var release = await _mediator.Send(new GetReleaseStateByIdForUserQuery(releaseId, User.Claims.GetUserId()));

            if (release == null)
                return BadRequest($"The release: {releaseId} does not exist");

            if (release == Domain.Enums.ReleaseStatus.Published)
                return BadRequest($"Cannot add tracks to a published release");

            var artist = await _mediator.Send(new GetArtistsByIdQuery(new[] { command.ArtistId }));

            if (artist == null)
                return BadRequest($"The artist: {command.ArtistId} does not exist");

            var genre = await _mediator.Send(new GetGenresByIdQuery(new[] { command.GenreId }));

            if (genre == null)
                return BadRequest($"The genre: {command.GenreId} does not exist");

            var trackId = Guid.NewGuid();
            command.SetReleaseId(releaseId);
            command.SetTrackId(trackId);
            await _mediator.Send(command);
            return Created(new Uri($"{Request.Host}{Request.Path}/{releaseId}/track/{trackId}"), trackId);
        }

        [HttpGet]
        [Route("releases/{releaseId}/track/{trackId}")]
        public async Task<IActionResult> GetTrack([FromRoute] Guid releaseId, [FromRoute] Guid trackId)
        {
            if (!ModelState.IsValid)
                return BadRequest($@"The field(s) {string.Join(", ", ModelState
                    .Where(e => e.Value.ValidationState == ModelValidationState.Invalid).Select(e => e.Key))} are not valid");

            var releaseTracks = await _mediator.Send(new GetTracksIdOfReleaseForUserQuery(releaseId, User.Claims.GetUserId()));

            if (releaseTracks == null || !releaseTracks.Any())
                return BadRequest($"The release: {releaseId} does not exist or does not have any tracks");

            var validTrackId = releaseTracks.Any(e => e == trackId);

            if (!validTrackId)
                return BadRequest($"The release: {releaseId} does not have the track {trackId}");

            return Ok(await _mediator.Send(new GetTracksByIdQuery(new[] { trackId })));
        }
    }
}