using System;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using DrakeLambert.Peerra.WebApi.SharedKernel.Interfaces.Infrastructure;
using DrakeLambert.Peerra.WebApi.IdentityCore.Entities;
using DrakeLambert.Peerra.WebApi.IdentityCore.Interfaces;
using DrakeLambert.Peerra.WebApi.IdentityCore.Interfaces.Infrastructure;
using DrakeLambert.Peerra.WebApi.SharedKernel.Dto;

namespace DrakeLambert.Peerra.WebApi.IdentityCore.Services
{
    public class UserTokenService : IUserTokenService
    {
        private readonly IIdentityUserPasswordService _userRepository;
        private readonly IRefreshTokenRepository _tokenRepository;
        private readonly ITokenFactory _tokenGenerator;
        private readonly ITokenValidator _tokenValidator;
        private readonly IAppLogger<UserTokenService> _logger;

        public UserTokenService(IIdentityUserPasswordService userRepository, IRefreshTokenRepository refreshTokenRepository, ITokenFactory tokenGenerator, ITokenValidator tokenValidator, IAppLogger<UserTokenService> logger)
        {
            _userRepository = userRepository;
            _tokenRepository = refreshTokenRepository;
            _tokenGenerator = tokenGenerator;
            _tokenValidator = tokenValidator;
            _logger = logger;
        }

        public async Task<Result<(AccessToken, RefreshToken)>> CreateAndSaveTokensAsync(string username, string password, string ipAddress)
        {
            Guard.Against.Null(username, nameof(username));
            Guard.Against.Null(password, nameof(password));
            Guard.Against.Null(ipAddress, nameof(ipAddress));

            _logger.LogInformation("Creating and saving tokens for user '{username}'.", username);

            var userCheck = await _userRepository.CheckPasswordAsync(username, password);

            if (userCheck.Failed)
            {
                _logger.LogWarning("Username and password check failed for user '{username}'", username);
                return Result<(AccessToken, RefreshToken)>.Fail("Invalid username or password.", userCheck);
            }

            // If password was incorrect
            if (!userCheck.Value)
            {
                _logger.LogWarning("Incorrect password for user '{username}'.", username);
                return Result<(AccessToken, RefreshToken)>.Fail("Invalid username or password.");
            }

            var accessToken = _tokenGenerator.GenerateAccessToken(username);
            var refreshToken = _tokenGenerator.GenerateRefreshToken(username, ipAddress);

            await _tokenRepository.AddRefreshTokenAsync(refreshToken);

            return Result<(AccessToken, RefreshToken)>.Success((accessToken, refreshToken));
        }

        public async Task<Result<(AccessToken, RefreshToken)>> ExchangeAndSaveTokensAsync(string accessToken, string refreshToken, string ipAddress)
        {
            Guard.Against.Null(accessToken, nameof(accessToken));
            Guard.Against.Null(refreshToken, nameof(refreshToken));
            Guard.Against.Null(ipAddress, nameof(ipAddress));

            _logger.LogInformation("Exchanging and saving tokens for user at IP address '{ipAddress}'.", ipAddress);

            var tokenCheck = _tokenValidator.ValidateAndGetUser(accessToken);

            if (tokenCheck.Failed)
            {
                _logger.LogWarning("Access token validation failed for user at IP address '{ipAddress}'.", ipAddress);
                return Result<(AccessToken, RefreshToken)>.Fail("Invalid access token.");
            }

            var username = tokenCheck.Value;

            var userRefreshTokens = await _tokenRepository.GetRefreshTokensAsync(username);

            var matchingToken = userRefreshTokens.FirstOrDefault(token => token.Token == refreshToken);

            if (matchingToken == null)
            {
                _logger.LogWarning("No matching refresh token for user '{username}'.", username);
                return Result<(AccessToken, RefreshToken)>.Fail("Invalid access token.");
            }

            if (matchingToken.Expiration < DateTimeOffset.Now)
            {
                _logger.LogWarning("Refresh token for user '{username}' is expired.", username);
                return Result<(AccessToken, RefreshToken)>.Fail("Invalid access token.");
            }

            if (matchingToken.IpAddress != ipAddress)
            {
                _logger.LogWarning("Refresh token for user '{username}' has IP address '{tokenIp}' while user has IP address '{userIp}'.", username, matchingToken.IpAddress, ipAddress);
                return Result<(AccessToken, RefreshToken)>.Fail("Invalid access token.");
            }

            var newAccessToken = _tokenGenerator.GenerateAccessToken(username);
            var newRefreshToken = _tokenGenerator.GenerateRefreshToken(username, ipAddress);

            await _tokenRepository.AddRefreshTokenAsync(newRefreshToken);
            var deleteOldTokenResult = await _tokenRepository.DeleteRefreshTokenAsync(matchingToken.Id);

            if (deleteOldTokenResult.Failed)
            {
                _logger.LogWarning("Failed to delete old token. Possible concurrency error. Application will continue.");
            }

            return Result<(AccessToken, RefreshToken)>.Success((newAccessToken, newRefreshToken));
        }
    }
}
