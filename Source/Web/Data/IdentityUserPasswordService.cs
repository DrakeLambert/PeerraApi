using System.Threading.Tasks;
using DrakeLambert.Peerra.WebApi.IdentityCore.Interfaces.Infrastructure;
using DrakeLambert.Peerra.WebApi.SharedKernel.Dto;
using DrakeLambert.Peerra.WebApi.Web.Entities;
using Microsoft.AspNetCore.Identity;

namespace DrakeLambert.Peerra.WebApi.Web.Data
{
    public class IdentityUserPasswordService : IIdentityUserPasswordService
    {
        private readonly UserManager<User> _userManager;

        public IdentityUserPasswordService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<bool>> CheckPasswordAsync(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null || !(await _userManager.CheckPasswordAsync(user, password)))
            {
                return Result<bool>.Fail("Username or password invalid.");
            }
            return Result<bool>.Success(true);
        }
    }
}
