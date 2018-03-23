using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IEnumerable<Domain.Entities.Release>> GetReleases()
        {
            return await _mediator.Send(new GetAllReleasesForUserQuery(User.Claims.GetUserId()));
        }

        [HttpPost]
        [Route("releases")]
        public async Task<IActionResult> AddRelease([FromBody] Domain.Commands.CreateReleaseCommand command)
        {
            var releaseId = Guid.NewGuid();
            //command.ReleaseId = releaseId;
            //command.UserId = User.Claims.GetUserId();

            await _mediator.Send(command);
            return Accepted(releaseId);
        }
    }
}
