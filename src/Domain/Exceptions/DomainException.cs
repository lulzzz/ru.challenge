using System;

namespace RU.Challenge.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(string errorMessage)
            : base(errorMessage)
        { }

        public DomainException(string errorMessage, Exception innerException)
            : base(errorMessage, innerException)
        { }
    }
}