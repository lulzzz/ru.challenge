using System.ComponentModel.DataAnnotations;

namespace RU.Challenge.Infrastructure.Identity.DTO
{
    public class LoginUser
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public LoginUser(
            string userName,
            string password)
        {
            UserName = userName;
            Password = password;
        }
    }
}