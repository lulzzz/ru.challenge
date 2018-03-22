using RU.Challenge.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace RU.Challenge.Infrastructure.Identity.Extensions
{
    public static class ClaimsExtensions
    {
        public static Guid GetUserId(this IEnumerable<Claim> @this)
        {
            var claim = @this.SingleOrDefault(e => e.Type == "uid");

            if (claim == null)
                throw new DomainException("The user id is not present on the JWT");

            return Guid.Parse(claim.Value);
        }

        public static IEnumerable<Claim> GetRoles(this IEnumerable<Claim> @this)
        {
            var claims = @this.Where(e => e.Type.Contains("identity/claims/role"));

            if (claims == null || !claims.Any())
                throw new DomainException("The roles are not present on the JWT");

            return claims;
        }

        public static string GetUserName(this IEnumerable<Claim> @this)
        {
            var claim = @this.SingleOrDefault(e => e.Type.Contains("identity/claims/nameidentifier"));

            if (claim == null)
                throw new DomainException("The user name is not present on the JWT");

            return claim.Value;
        }
    }
}