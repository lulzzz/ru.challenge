using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using RU.Challenge.Domain.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RU.Challenge.Presentation.API.Controllers
{
    [Route("api")]
    [Authorize(Roles = "api_access, api_release_manager")]
    public class PaymentMethodController : Controller
    {
        private readonly IMediator _mediator;

        public PaymentMethodController(IMediator mediator)
            => _mediator = mediator;

        [HttpGet]
        [Route("paymentmethod")]
        public async Task<IEnumerable<Domain.Entities.PaymentMethod>> GetPaymentMethods()
        {
            return await _mediator.Send(new GetAllPaymentMethodsQuery());
        }

        [HttpPost]
        [Route("paymentmethod")]
        public async Task<IActionResult> AddPaymentMethod([FromBody] Domain.Commands.CreatePaymentMethodCommand command)
        {
            await _mediator.Send(command);
            return Accepted();
        }
    }
}