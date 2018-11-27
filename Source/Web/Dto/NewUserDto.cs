using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DrakeLambert.Peerra.WebApi.Web.Dto
{
    public class NewUserDto : UserCredentialsDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Bio { get; set; }

        public IEnumerable<string> Skills { get; set; }
    }
}
