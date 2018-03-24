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
    public class SubscriptionController : Controller
    {
        private readonly IMediator _mediator;

        public SubscriptionController(IMediator mediator)
            => _mediator = mediator;

        [HttpGet]
        [Route("subscriptions")]
        public async Task<IEnumerable<Subscription>> GetSubscriptions()
        {
            return await _mediator.Send(new GetSubscriptionsByIdQuery(ids: null));
        }

        [HttpGet]
        [Route("subscriptions/id/{id}")]
        public async Task<IActionResult> GetSubscriptionById([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest($@"The field(s) {string.Join(", ", ModelState
                    .Where(e => e.Value.ValidationState == ModelValidationState.Invalid).Select(e => e.Key))} are not valid");

            return Ok((await _mediator.Send(new GetSubscriptionsByIdQuery(new[] { id }))).FirstOrDefault());
        }

        [HttpPost]
        [Route("subscriptions")]
        public async Task<IActionResult> AddSubscription([FromBody] CreateSubscriptionCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest($@"The field(s) {string.Join(", ", ModelState
                    .Where(e => e.Value.ValidationState == ModelValidationState.Invalid).Select(e => e.Key))} are not valid");

            var invalidDistributionPlatforms = await GetInvalidDistributionPlatforms(command.DistributionPlatformIds);

            if (invalidDistributionPlatforms.Any())
                return BadRequest($"The distribution platforms: {string.Join(", ", invalidDistributionPlatforms.Select(e => e.ToString()))} do not exist");

            var paymentMethod = await _mediator.Send(new GetPaymentMethodsByIdQuery(new[] { command.PaymentMethodId }));

            if (paymentMethod == null)
                return BadRequest($"The payment method: {command.PaymentMethodId} does not exist");

            var subscriptionId = Guid.NewGuid();
            command.SetId(subscriptionId);
            await _mediator.Send(command);
            return Created(new Uri($"{Request.Host}{Request.Path}/id/{subscriptionId}"), subscriptionId);
        }

        [HttpPost]
        [Route("subscriptions/{subscriptionId}/adddistributionplatform/{distributionPlatformId}")]
        public async Task<IActionResult> AddDistributionPlatform(
            [FromRoute] Guid subscriptionId, [FromRoute] Guid distributionPlatformId)
        {
            if (!ModelState.IsValid)
                return BadRequest($@"The field(s) {string.Join(", ", ModelState
                    .Where(e => e.Value.ValidationState == ModelValidationState.Invalid).Select(e => e.Key))} are not valid");

            var existSubscription = await _mediator.Send(new ExistsSubscriptionByIdQuery(subscriptionId));

            if (existSubscription == false)
                return BadRequest($"The subscription: {subscriptionId} does not exist");

            var invalidDistributionPlatforms = await GetInvalidDistributionPlatforms(new[] { distributionPlatformId });

            if (invalidDistributionPlatforms.Any())
                return BadRequest($"The distribution platforms: {string.Join(", ", invalidDistributionPlatforms.Select(e => e.ToString()))} do not exist");

            await _mediator.Send(new AddDistributionPlatformToSubscriptionCommand(subscriptionId, distributionPlatformId));
            return Accepted();
        }

        private async Task<IEnumerable<Guid>> GetInvalidDistributionPlatforms(IEnumerable<Guid> distributionPlatformIds)
        {
            var distributionPlatforms =
                (await _mediator.Send(new GetDistributionPlatformsByIdQuery(distributionPlatformIds)))
                .Select(e => e.Id);

            return distributionPlatformIds.Where(e => !distributionPlatforms.Contains(e));
        }
    }
}