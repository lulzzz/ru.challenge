using System.ComponentModel.DataAnnotations;

namespace RU.Challenge.Infrastructure.Identity.DTO
{
    public class LoginUser
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public LoginUser(
            string email,
            string password)
        {
            Email = email;
            Password = password;
        }
    }
}