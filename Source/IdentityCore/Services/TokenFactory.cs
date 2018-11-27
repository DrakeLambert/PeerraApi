using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Ardalis.GuardClauses;
using DrakeLambert.Peerra.WebApi.SharedKernel.Interfaces.Infrastructure;
using DrakeLambert.Peerra.WebApi.IdentityCore.Entities;
using DrakeLambert.Peerra.WebApi.IdentityCore.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DrakeLambert.Peerra.WebApi.IdentityCore.Services
{
    public class TokenFactory : ITokenFactory
    {
        private readonly AuthOptions _options;
        private readonly IAppLogger<TokenFactory> _logger;

        public TokenFactory(IOptions<AuthOptions> options, IAppLogger<TokenFactory> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public AccessToken GenerateAccessToken(string username)
        {
            Guard.Against.Null(username, nameof(username));

            _logger.LogInformation("Generating access token for {username}.", username);

            var expiration = DateTimeOffset.Now.Add(_options.AccessTokenLifetime);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                            {
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = expiration.UtcDateTime,
                SigningCredentials = new SigningCredentials(_options.SecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            return new AccessToken
            {
                Token = tokenHandler.CreateEncodedJwt(tokenDescriptor),
                Expiration = expiration
            };
        }

        public RefreshToken GenerateRefreshToken(string username, string ipAddress)
        {
            Guard.Against.Null(username, nameof(username));
            Guard.Against.Null(ipAddress, nameof(ipAddress));

            _logger.LogInformation("Generating refresh token for {username} at address {ipAddress}.", username, ipAddress);

            var randomNumber = new byte[32];
            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(randomNumber);
            }

            return new RefreshToken
            {
                Expiration = DateTimeOffset.Now.Add(_options.AccessTokenLifetime),
                IpAddress = ipAddress,
                Username = username,
                Token = Convert.ToBase64String(randomNumber)
            };
        }
    }
}
