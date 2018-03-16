using System;

namespace RU.Challenge.Domain.Entities
{
    public class PaymentMethod
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; }

        private PaymentMethod(Guid id, string name) : this()
        {
            Id = id;
            Name = name;
        }

        private PaymentMethod()
        {
        }

        public static PaymentMethod Create(string name)
            => new PaymentMethod(Guid.NewGuid(), name);
    }
}