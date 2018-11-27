using System.Threading.Tasks;
using DrakeLambert.Peerra.WebApi.IdentityCore.Entities;
using DrakeLambert.Peerra.WebApi.SharedKernel.Dto;

namespace DrakeLambert.Peerra.WebApi.IdentityCore.Interfaces
{
    public interface IUserTokenService
    {
        Task<Result<(AccessToken, RefreshToken)>> CreateAndSaveTokensAsync(string username, string password, string ipAddress);

        Task<Result<(AccessToken, RefreshToken)>> ExchangeAndSaveTokensAsync(string accessToken, string refreshToken, string ipAddress);
    }
}
