using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using RU.Challenge.Domain.Commands;
using RU.Challenge.Domain.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RU.Challenge.Presentation.API.Controllers
{
    [Route("api")]
    public class SubscriptionController : Controller
    {
        private readonly IMediator _mediator;

        public SubscriptionController(IMediator mediator)
            => _mediator = mediator;

        [HttpGet]
        [Route("subscription")]
        public async Task<IEnumerable<Domain.Entities.Subscription>> GetSubscriptions()
        {
            return await _mediator.Send(new GetAllSubscriptionsQuery());
        }

        [HttpPost]
        [Route("subscription")]
        public async Task<IActionResult> AddSubscription([FromBody] Domain.Commands.CreateSubscriptionCommand command)
        {
            var distributionPlatforms =
                (await _mediator.Send(new GetDistributionPlatformsByIdQuery(command.DistributionPlatformIds)))
                .Select(e => e.Id);

            var notExists = command.DistributionPlatformIds.Where(e => !distributionPlatforms.Contains(e));

            if (!command.DistributionPlatformIds.Any(e => distributionPlatforms.Contains(e)))
                return BadRequest($"The distribution platforms: {string.Join(", ", notExists.Select(e => e.ToString()))} do not exist");

            var platformId = await _mediator.Send(new GetPaymentMethodByIdQuery(command.PaymentMethodId));

            if (platformId == null)
                return BadRequest($"The platform: {command.PaymentMethodId} does not exist");

            await _mediator.Send(command);
            return Accepted();
        }

        [HttpPost]
        [Route("subscription/{subscriptionId}/adddistributionplatform/{distributionPlatformId}")]
        public async Task<IActionResult> AddDistributionPlatform(
            [FromRoute] Guid subscriptionId, [FromRoute] Guid distributionPlatformId)
        {
            await _mediator.Send(new AddDistributionPlatformToSubscriptionCommand(subscriptionId, distributionPlatformId));
            return Accepted();
        }
    }
}