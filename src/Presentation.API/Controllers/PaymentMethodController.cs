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
    public class PaymentMethodController : Controller
    {
        private readonly IMediator _mediator;

        public PaymentMethodController(IMediator mediator)
            => _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        [HttpGet]
        [Route("paymentmethods")]
        public async Task<IActionResult> GetPaymentMethods()
        {
            var items = await _mediator.Send(new GetPaymentMethodsByIdQuery(ids: null));

            if (items == null || !items.Any())
                return NotFound();
            else
                return Ok(items);
        }

        [HttpGet]
        [Route("paymentmethods/id/{id}")]
        public async Task<IActionResult> GetPaymentMethodById([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest($@"The field(s) {string.Join(", ", ModelState
                    .Where(e => e.Value.ValidationState == ModelValidationState.Invalid).Select(e => e.Key))} are not valid");

            var item = (await _mediator.Send(new GetPaymentMethodsByIdQuery(new[] { id }))).FirstOrDefault();

            if (item == null)
                return NotFound();
            else
                return Ok(item);
        }

        [HttpPost]
        [Route("paymentmethods")]
        public async Task<IActionResult> AddPaymentMethod([FromBody] CreatePaymentMethodCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest($@"The field(s) {string.Join(", ", ModelState
                    .Where(e => e.Value.ValidationState == ModelValidationState.Invalid).Select(e => e.Key))} are not valid");

            var paymentMethodId = Guid.NewGuid();
            command.SetId(paymentMethodId);
            await _mediator.Send(command);
            return Created($"{Request.Host}{Request.Path}/id/{paymentMethodId}", paymentMethodId);
        }
    }
}