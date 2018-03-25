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
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _fileUploader = fileUploader ?? throw new ArgumentNullException(nameof(fileUploader));
        }

        [HttpGet]
        [Route("releases")]
        public async Task<IActionResult> GetReleases()
        {
            var items = await _mediator.Send(new GetReleasesByIdForUserQuery(ids: null, userId: User.Claims.GetUserId()));

            if (items == null || !items.Any())
                return NotFound();
            else
                return Ok(items);
        }

        [HttpGet]
        [Route("releases/id/{id}")]
        public async Task<IActionResult> GetReleseById([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest($@"The field(s) {string.Join(", ", ModelState
                    .Where(e => e.Value.ValidationState == ModelValidationState.Invalid).Select(e => e.Key))} are not valid");

            var item = (await _mediator.Send(new GetReleasesByIdForUserQuery(ids: new[] { id }, userId: User.Claims.GetUserId()))).FirstOrDefault();

            if (item == null)
                return NotFound();
            else
                return Ok(item);
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

            return Created($"{Request.Host}/releases/id/{releaseId}", releaseId);
        }

        [HttpPost]
        [Route("releases/{releaseId}/track/{name}/{artistId}/{genreId}")]
        public async Task<IActionResult> AddTrackToRelease(IFormFile song,
            [FromRoute] Guid releaseId, [FromRoute] string name, [FromRoute] Guid artistId, [FromRoute] Guid genreId)
        {
            if (!ModelState.IsValid)
                return BadRequest($@"The field(s) {string.Join(", ", ModelState
                    .Where(e => e.Value.ValidationState == ModelValidationState.Invalid).Select(e => e.Key))} are not valid");

            if (!song.ContentType.Contains("audio"))
                return BadRequest($"The uploaded file is not an image");

            var releaseState = await _mediator.Send(new GetReleaseStateByIdForUserQuery(releaseId, User.Claims.GetUserId()));

            if (releaseState == null)
                return BadRequest($"The release: {releaseId} does not exist");

            if (releaseState == Domain.Enums.ReleaseStatus.Published)
                return BadRequest($"Cannot add tracks to a published release");

            var artist = await _mediator.Send(new GetArtistsByIdQuery(new[] { artistId }));

            if (artist == null)
                return BadRequest($"The artist: {artistId} does not exist");

            var genre = await _mediator.Send(new GetGenresByIdQuery(new[] { genreId }));

            if (genre == null)
                return BadRequest($"The genre: {genreId} does not exist");

            var trackId = Guid.NewGuid();
            var songUrl = await _fileUploader.UploadFileAsync(song.FileName, song.ContentType, song.OpenReadStream());
            var command = new CreateTrackCommand(name, songUrl, genreId, artistId, releaseId, trackId);
            await _mediator.Send(command);
            return Created($"{Request.Host}/releases/{releaseId}/track/{trackId}", trackId);
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

            var item = (await _mediator.Send(new GetTracksByIdQuery(new[] { trackId }))).FirstOrDefault();

            if (item == null)
                return NotFound();
            else
                return Ok(item);
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