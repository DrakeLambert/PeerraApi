using System.Linq;
using System.Threading.Tasks;
using DrakeLambert.Peerra.WebApi.SharedKernel.Interfaces.Infrastructure;
using DrakeLambert.Peerra.WebApi.IdentityCore.Interfaces;
using DrakeLambert.Peerra.WebApi.Web.Dto;
using DrakeLambert.Peerra.WebApi.Web.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DrakeLambert.Peerra.WebApi.Web.Data;

namespace DrakeLambert.Peerra.WebApi.Web.Controllers
{
    public class AccountController : ApiControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserTokenService _tokenService;
        private readonly ApplicationDbContext _context;
        private readonly IAppLogger<AccountController> _logger;

        public AccountController(UserManager<User> userManager, IUserTokenService tokenService, ApplicationDbContext context, IAppLogger<AccountController> logger)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="newUser">The new user's information.</param>
        /// <returns>The new user or an error.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(UserInfoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] NewUserDto newUser)
        {
            _logger.LogInformation("Registering user '{username}'.", newUser.Username);

            var appUser = new User
            {
                UserName = newUser.Username,
                Bio = newUser.Bio,
                Email = newUser.Email
            };

            var addUserResult = await _userManager.CreateAsync(appUser, newUser.Password);

            if (!addUserResult.Succeeded)
            {
                _logger.LogWarning("New user with name '{username}', was invalid.", newUser.Username);
                return BadRequest(new ErrorDto(addUserResult.Errors.Select(error => error.Description).FirstOrDefault()));
            }

            if (newUser.Skills != null)
            {
                _context.UserSkills.AddRange(newUser.Skills.Select(skill => new UserSkill
                {
                    SkillName = skill,
                    Username = newUser.Username
                }));
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(Info), new UserInfoDto(appUser));
        }

        /// <summary>
        /// Gets the user info for the currently logged in user.
        /// </summary>
        /// <returns>The user's info or an error.</returns>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(UserInfoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Info()
        {
            var username = User.Identity.Name;
            _logger.LogInformation("Retrieving info for user '{username}'.", username);

            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return BadRequest(new ErrorDto("Username not found."));
            }

            return Ok(new UserInfoDto(user));
        }

        /// <summary>
        /// Creates tokens for a user.
        /// </summary>
        /// <param name="userCredentials">The user's credentials.</param>
        /// <returns>The new tokens or an error.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(TokenDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Access([FromBody] UserCredentialsDto userCredentials)
        {
            _logger.LogInformation("Generating tokens for user '{username}'.", userCredentials.Username);

            var tokenResult = await _tokenService.CreateAndSaveTokensAsync(userCredentials.Username, userCredentials.Password, HttpContext.Connection.RemoteIpAddress.ToString());

            if (tokenResult.Failed)
            {
                _logger.LogWarning("Token generation for user '{username}' failed.", userCredentials.Username);
                return BadRequest(new ErrorDto(tokenResult));
            }

            return StatusCode(StatusCodes.Status201Created, new TokenDto(tokenResult.Value));
        }

        /// <summary>
        /// Exchanges tokens given a valid refresh token.
        /// </summary>
        /// <param name="plainTokens">The expired access and valid refresh token.</param>
        /// <returns>The new tokens or an error.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(TokenDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Refresh([FromBody] PlainTokenDto plainTokens)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();

            _logger.LogInformation("Refreshing tokens for user at IP address '{ipAddress}'.", ipAddress);

            var tokenExchangeResult = await _tokenService.ExchangeAndSaveTokensAsync(plainTokens.AccessToken, plainTokens.RefreshToken, ipAddress);

            if (tokenExchangeResult.Failed)
            {
                _logger.LogWarning("Token exchange for user at IP address '{ipAddress}' failed.", ipAddress);
                return BadRequest(new ErrorDto(tokenExchangeResult));
            }

            return StatusCode(StatusCodes.Status201Created, new TokenDto(tokenExchangeResult.Value));
        }
    }
}
