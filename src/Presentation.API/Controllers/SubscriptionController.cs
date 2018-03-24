﻿using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            return await _mediator.Send(new GetAllSubscriptionsQuery());
        }

        [HttpGet]
        [Route("subscriptions/id/{id}")]
        public async Task<Subscription> GetSubscriptionById([FromRoute] Guid id)
        {
            return await _mediator.Send(new GetSubscriptionByIdQuery(id));
        }

        [HttpPost]
        [Route("subscriptions")]
        public async Task<IActionResult> AddSubscription([FromBody] CreateSubscriptionCommand command)
        {
            var invalidDistributionPlatforms = await GetInvalidDistributionPlatforms(command.DistributionPlatformIds);

            if (invalidDistributionPlatforms.Any())
                return BadRequest($"The distribution platforms: {string.Join(", ", invalidDistributionPlatforms.Select(e => e.ToString()))} do not exist");

            var paymentMethod = await _mediator.Send(new GetPaymentMethodByIdQuery(command.PaymentMethodId));

            if (paymentMethod == null)
                return BadRequest($"The payment method: {command.PaymentMethodId} does not exist");

            var subscriptionId = Guid.NewGuid();
            command.SetId(subscriptionId);
            await _mediator.Send(command);
            return Created(new Uri($"{Request.Host}{Request.Path}/id/{subscriptionId}"), subscriptionId);
        }

        [HttpPost]
        [Route("subscription/{subscriptionId}/adddistributionplatform/{distributionPlatformId}")]
        public async Task<IActionResult> AddDistributionPlatform(
            [FromRoute] Guid subscriptionId, [FromRoute] Guid distributionPlatformId)
        {
            var subscription = await _mediator.Send(new GetSubscriptionByIdQuery(subscriptionId));

            if (subscription == null)
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