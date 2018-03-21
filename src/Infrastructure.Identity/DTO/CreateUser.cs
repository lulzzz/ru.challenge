using System.ComponentModel.DataAnnotations;

namespace RU.Challenge.Infrastructure.Identity.DTO
{
    public class CreateUser
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public CreateUser(
            string email,
            string username,
            string password)
        {
            Email = email;
            Username = username;
            Password = password;
        }
    }
}