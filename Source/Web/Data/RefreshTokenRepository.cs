using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DrakeLambert.Peerra.WebApi.IdentityCore.Entities;
using DrakeLambert.Peerra.WebApi.IdentityCore.Interfaces.Infrastructure;
using DrakeLambert.Peerra.WebApi.SharedKernel.Dto;
using Microsoft.EntityFrameworkCore;

namespace DrakeLambert.Peerra.WebApi.Web.Data
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context;

        public RefreshTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task<Result> DeleteRefreshTokenAsync(Guid id)
        {
            var token = await _context.RefreshTokens.FindAsync(id);
            if (token == null)
            {
                return Result.Fail("Token does not exist.");
            }
            _context.RefreshTokens.Remove(token);
            await _context.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<List<RefreshToken>> GetRefreshTokensAsync(string username)
        {
            return await _context.RefreshTokens.Where(token => token.Username == username).ToListAsync();
        }
    }
}
