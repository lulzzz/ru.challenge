using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using RU.Challenge.Domain.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RU.Challenge.Presentation.API.Controllers
{
    [Route("api")]
    public class DistributionPlatformController : Controller
    {
        private readonly IMediator _mediator;

        public DistributionPlatformController(IMediator mediator)
            => _mediator = mediator;

        [HttpGet]
        [Route("distributionplatform")]
        public async Task<IEnumerable<Domain.Entities.DistributionPlatform>> GetDistributionPlatforms()
        {
            return await _mediator.Send(new GetAllDistributionPlatformsQuery());
        }

        [HttpPost]
        [Route("distributionplatform")]
        public async Task<IActionResult> AddDistributionPlatform([FromBody] Domain.Commands.CreateDistributionPlatformCommand command)
        {
            await _mediator.Send(command);
            return Accepted();
        }
    }
}