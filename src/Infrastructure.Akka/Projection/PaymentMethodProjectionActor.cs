﻿using Akka.Actor;
using Infrastructure.Repositories;
using RU.Challenge.Domain.Events;

namespace RU.Challenge.Infrastructure.Akka.Projection
{
    public class PaymentMethodProjectionActor : ReceiveActor
    {
        public PaymentMethodProjectionActor(IPaymentMethodRepository paymentMethodRepository)
        {
            Receive<CreatePaymentMethodEvent>(e => paymentMethodRepository.AddAsync(e));
        }

        protected override void PreStart()
        {
            Context.System.EventStream.Subscribe(Self, typeof(CreatePaymentMethodEvent));
        }
    }
}