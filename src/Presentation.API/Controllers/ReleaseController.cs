using Google.Cloud.Storage.V1;
using Infrastructure.Uploaders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RU.Challenge.Domain.Commands;
using RU.Challenge.Domain.Entities;
using RU.Challenge.Domain.Queries;
using RU.Challenge.Infrastructure.Identity.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RU.Challenge.Presentation.API.Controllers
{
    [Route("api")]
    [Authorize(Roles = "ReleaseManager")]
    public class ReleaseController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IFileUploader _fileUploader;

        public ReleaseController(IMediator mediator, IFileUploader fileUploader)
        {
            _mediator = mediator;
            _fileUploader = fileUploader;
        }

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
        [Route("releases/{title}/{artistId}/{genreId}")]
        public async Task<IActionResult> AddRelease(IFormFile coverArt, [FromRoute] string title, [FromRoute] Guid artistId, [FromRoute] Guid genreId)
        {
            if (!coverArt.ContentType.Contains("image"))
                return BadRequest($"The uploaded file is not an image");

            var artist = await _mediator.Send(new GetArtistsByIdQuery(new[] { artistId }));

            if (artist == null)
                return BadRequest($"The artist: {artistId} does not exist");

            var genre = await _mediator.Send(new GetGenresByIdQuery(new[] { genreId }));

            if (genre == null)
                return BadRequest($"The genre: {genreId} does not exist");

            var releaseId = Guid.NewGuid();
            var coverArtUrl = await _fileUploader.UploadFileAsync(coverArt.FileName, coverArt.ContentType, coverArt.OpenReadStream());
            var command = new CreateReleaseCommand(title, artistId, genreId, releaseId, User.Claims.GetUserId(), coverArtUrl);
            await _mediator.Send(command);
            return Created(new Uri($"{Request.Host}/releases/id/{releaseId}"), releaseId);
        }

        [HttpPost]
        [Route("releases/{releaseId}/track")]
        public async Task<IActionResult> AddTrackToRelease([FromRoute] Guid releaseId, [FromBody] CreateTrackCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest($@"The field(s) {string.Join(", ", ModelState
                    .Where(e => e.Value.ValidationState == ModelValidationState.Invalid).Select(e => e.Key))} are not valid");

            var releaseState = await _mediator.Send(new GetReleaseStateByIdForUserQuery(releaseId, User.Claims.GetUserId()));

            if (releaseState == null)
                return BadRequest($"The release: {releaseId} does not exist");

            if (releaseState == Domain.Enums.ReleaseStatus.Published)
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

        [HttpPost]
        [Route("releases/{releaseId}/subscription/{subscriptionId}")]
        public async Task<IActionResult> AddSubscriptionToRelease([FromRoute] Guid releaseId, [FromRoute] Guid subscriptionId)
        {
            if (!ModelState.IsValid)
                return BadRequest($@"The field(s) {string.Join(", ", ModelState
                    .Where(e => e.Value.ValidationState == ModelValidationState.Invalid).Select(e => e.Key))} are not valid");

            var releaseState = await _mediator.Send(new GetReleaseStateByIdForUserQuery(releaseId, User.Claims.GetUserId()));

            if (releaseState == null)
                return BadRequest($"The release: {releaseId} does not exist");

            var existSubscription = await _mediator.Send(new ExistsSubscriptionByIdQuery(subscriptionId));

            if (existSubscription == false)
                return BadRequest($"The subscription: {subscriptionId} does not exist");

            var subscription = await _mediator.Send(new GetReleaseSubscriptionByIdForUserQuery(releaseId, User.Claims.GetUserId()));

            if (releaseState == Domain.Enums.ReleaseStatus.Published &&
                (subscription != null && subscription.ExpirationDate > DateTimeOffset.Now))
                return BadRequest($"The release: {releaseId} has an active subscription until {subscription.ExpirationDate.ToString("dd/MM/yyyy HH:mm")}");

            await _mediator.Send(new AddSubscriptionToReleaseCommand(releaseId, subscriptionId));
            return Accepted();
        }
    }
}