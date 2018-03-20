using System;

namespace RU.Challenge.Domain.Entities.Auth
{
    public class Role
    {
        public Guid Id { get; set; }

        public string RoleName { get; set; }

        public string NormalizedRoleName { get; set; }
    }
}