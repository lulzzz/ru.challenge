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

        private PaymentMethod(PaymentMethod other) : this()
        {
            Id = other.Id;
            Name = other.Name;
        }

        private PaymentMethod()
        {
        }

        public PaymentMethod Clone() => new PaymentMethod(this);

        public static PaymentMethod Create(Guid id, string name)
            => new PaymentMethod(id, name);
    }
}