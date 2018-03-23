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
    public class PaymentMethodController : Controller
    {
        private readonly IMediator _mediator;

        public PaymentMethodController(IMediator mediator)
            => _mediator = mediator;

        [HttpGet]
        [Route("paymentmethods")]
        public async Task<IEnumerable<Domain.Entities.PaymentMethod>> GetPaymentMethods()
        {
            return await _mediator.Send(new GetAllPaymentMethodsQuery());
        }

        [HttpPost]
        [Route("paymentmethods")]
        public async Task<IActionResult> AddPaymentMethod([FromBody] Domain.Commands.CreatePaymentMethodCommand command)
        {
            var paymentMethodId = Guid.NewGuid();
            command.PaymentMethodId = paymentMethodId;
            await _mediator.Send(command);
            return Accepted(paymentMethodId);
        }
    }
}