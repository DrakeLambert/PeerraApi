using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DrakeLambert.Peerra.WebApi.IdentityCore.Entities;
using DrakeLambert.Peerra.WebApi.SharedKernel.Dto;

namespace DrakeLambert.Peerra.WebApi.IdentityCore.Interfaces.Infrastructure
{
    public interface IRefreshTokenRepository
    {
        Task<List<RefreshToken>> GetRefreshTokensAsync(string username);

        Task AddRefreshTokenAsync(RefreshToken refreshToken);

        Task<Result> DeleteRefreshTokenAsync(Guid id);
    }
}
