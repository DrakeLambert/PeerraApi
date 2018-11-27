using System.ComponentModel.DataAnnotations;

namespace DrakeLambert.Peerra.WebApi.Web.Dto
{
    public class NewConnectionDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Message { get; set; }
    }
}
