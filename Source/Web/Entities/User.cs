using Microsoft.AspNetCore.Identity;

namespace DrakeLambert.Peerra.WebApi.Web.Entities
{
    public class User : IdentityUser
    {
        public string Bio { get; set; }
    }
}
