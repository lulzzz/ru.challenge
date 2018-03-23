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
    public class DistributionPlatformController : Controller
    {
        private readonly IMediator _mediator;

        public DistributionPlatformController(IMediator mediator)
            => _mediator = mediator;

        [HttpGet]
        [Route("distributionplatforms")]
        public async Task<IEnumerable<Domain.Entities.DistributionPlatform>> GetDistributionPlatforms()
        {
            return await _mediator.Send(new GetAllDistributionPlatformsQuery());
        }

        [HttpPost]
        [Route("distributionplatforms")]
        public async Task<IActionResult> AddDistributionPlatform([FromBody] Domain.Commands.CreateDistributionPlatformCommand command)
        {
            var distributionPlatformId = Guid.NewGuid();
            command.DistributionPlatformId = distributionPlatformId;
            await _mediator.Send(command);
            return Accepted(distributionPlatformId);
        }
    }
}