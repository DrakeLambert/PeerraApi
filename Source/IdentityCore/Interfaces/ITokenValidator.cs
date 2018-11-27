using DrakeLambert.Peerra.WebApi.SharedKernel.Dto;

namespace DrakeLambert.Peerra.WebApi.IdentityCore.Interfaces
{
    public interface ITokenValidator
    {
        Result<string> ValidateAndGetUser(string accessToken);
    }
}
