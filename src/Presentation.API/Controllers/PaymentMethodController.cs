using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using RU.Challenge.Domain.Commands;
using RU.Challenge.Domain.Entities;
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
        public async Task<IEnumerable<PaymentMethod>> GetPaymentMethods()
        {
            return await _mediator.Send(new GetAllPaymentMethodsQuery());
        }

        [HttpGet]
        [Route("paymentmethods/id/{id}")]
        public async Task<PaymentMethod> GetPaymentMethodById([FromRoute] Guid id)
        {
            return await _mediator.Send(new GetPaymentMethodByIdQuery(id));
        }

        [HttpPost]
        [Route("paymentmethods")]
        public async Task<IActionResult> AddPaymentMethod([FromBody] CreatePaymentMethodCommand command)
        {
            var paymentMethodId = Guid.NewGuid();
            command.SetId(paymentMethodId);
            await _mediator.Send(command);
            return Created(new Uri($"{Request.Host}{Request.Path}/id/{paymentMethodId}"), paymentMethodId);
        }
    }
}