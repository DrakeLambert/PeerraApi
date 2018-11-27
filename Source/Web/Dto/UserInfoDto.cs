using DrakeLambert.Peerra.WebApi.Web.Entities;

namespace DrakeLambert.Peerra.WebApi.Web.Dto
{
    public class UserInfoDto
    {
        public string Username { get; set; }
        public string Bio { get; set; }
        public string Email { get; set; }

        public UserInfoDto(User user)
        {
            Username = user.UserName;
            Bio = user.Bio;
            Email = user.Email;
        }

        public UserInfoDto()
        { }
    }
}
