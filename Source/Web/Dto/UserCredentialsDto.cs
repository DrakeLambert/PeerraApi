using System.ComponentModel.DataAnnotations;

namespace DrakeLambert.Peerra.WebApi.Web.Dto
{
    public class UserCredentialsDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
