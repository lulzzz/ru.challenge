using System.ComponentModel.DataAnnotations;

namespace RU.Challenge.Infrastructure.Identity.DTO
{
    public class User
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public User(
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