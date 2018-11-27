using System.Threading.Tasks;
using DrakeLambert.Peerra.WebApi.SharedKernel.Dto;

namespace DrakeLambert.Peerra.WebApi.IdentityCore.Interfaces.Infrastructure
{
    public interface IIdentityUserPasswordService
    {
        Task<Result<bool>> CheckPasswordAsync(string username, string password);
    }
}
