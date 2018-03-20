using System;

namespace RU.Challenge.Infrastructure.Identity.DTO
{
    public class TokenResponse
    {
        public Guid UserId { get; }

        public string AuthToken { get; }

        public long ExpirationSeconds { get; }

        public TokenResponse(
            Guid userId,
            string authToken,
            long expirationSeconds)
        {
            UserId = userId;
            AuthToken = authToken;
            ExpirationSeconds = expirationSeconds;
        }
    }
}