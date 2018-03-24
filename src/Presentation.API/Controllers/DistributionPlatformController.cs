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
    public class DistributionPlatformController : Controller
    {
        private readonly IMediator _mediator;

        public DistributionPlatformController(IMediator mediator)
            => _mediator = mediator;

        [HttpGet]
        [Route("distributionplatforms")]
        public async Task<IEnumerable<DistributionPlatform>> GetDistributionPlatforms()
        {
            return await _mediator.Send(new GetDistributionPlatformsByIdQuery(ids: null));
        }

        [HttpGet]
        [Route("distributionplatforms/id/{id}")]
        public async Task<IActionResult> GetDistributionPlatformById([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest($@"The field(s) {string.Join(", ", ModelState
                    .Where(e => e.Value.ValidationState == ModelValidationState.Invalid).Select(e => e.Key))} are not valid");

            return Ok((await _mediator.Send(new GetDistributionPlatformsByIdQuery(new[] { id }))).FirstOrDefault());
        }

        [HttpPost]
        [Route("distributionplatforms")]
        public async Task<IActionResult> AddDistributionPlatform([FromBody] CreateDistributionPlatformCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest($@"The field(s) {string.Join(", ", ModelState
                    .Where(e => e.Value.ValidationState == ModelValidationState.Invalid).Select(e => e.Key))} are not valid");

            var distributionPlatformId = Guid.NewGuid();
            command.SetId(distributionPlatformId);
            await _mediator.Send(command);
            return Created(new Uri($"{Request.Host}{Request.Path}/id/{distributionPlatformId}"), distributionPlatformId);
        }
    }
}