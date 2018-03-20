using System;

namespace RU.Challenge.Domain.Entities.Auth
{
    public class User
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string NormalizedUserName { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }
    }
}