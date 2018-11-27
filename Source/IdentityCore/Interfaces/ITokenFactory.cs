using DrakeLambert.Peerra.WebApi.IdentityCore.Entities;

namespace DrakeLambert.Peerra.WebApi.IdentityCore.Interfaces
{
    public interface ITokenFactory
    {
        AccessToken GenerateAccessToken(string username);

        RefreshToken GenerateRefreshToken(string username, string ipAddress);
    }
}
