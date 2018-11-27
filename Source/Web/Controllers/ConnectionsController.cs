using System.Linq;
using System.Threading.Tasks;
using DrakeLambert.Peerra.WebApi.Web.Data;
using DrakeLambert.Peerra.WebApi.Web.Dto;
using DrakeLambert.Peerra.WebApi.Web.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DrakeLambert.Peerra.WebApi.Web.Controllers
{
    [Authorize]
    public class ConnectionsController : ApiControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public ConnectionsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Connect([FromBody] NewConnectionDto newConnection)
        {
            var targetUser = await _userManager.FindByNameAsync(newConnection.Username);
            var requestorUsername = User.Identity.Name;

            if (targetUser == null)
            {
                return BadRequest(new ErrorDto("The user does not exist."));
            }

            var connectionExists = await _context.Connections.Where(conneciton => conneciton.RequestorUsername == requestorUsername && conneciton.TargetUsername == targetUser.UserName).CountAsync() > 0;

            if (connectionExists)
            {
                return BadRequest(new ErrorDto("You are already connected to this user."));
            }

            var connection = new Connection
            {
                Accepted = false,
                Declined = false,
                Message = newConnection.Message,
                RequestorUsername = requestorUsername,
                TargetUsername = targetUser.UserName
            };

            _context.Connections.Add(connection);

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Connections()
        {
            var requestorUsername = User.Identity.Name;

            var connections = await _context.Connections.Where(connection => connection.RequestorUsername == requestorUsername).ToListAsync();

            return Ok(connections);
        }

        [HttpGet]
        public async Task<IActionResult> Connection([FromQuery] string username)
        {
            if (username == null)
            {
                return BadRequest(new ErrorDto("A username must be specified."));
            }

            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return BadRequest(new ErrorDto("The user does not exist."));
            }

            var targetUsername = User.Identity.Name;

            var connection = await _context.Connections.FirstOrDefaultAsync(c => (c.RequestorUsername == username || c.RequestorUsername == targetUsername) && (c.TargetUsername == targetUsername || c.TargetUsername == username));

            if (connection == null)
            {
                return BadRequest(new ErrorDto("You are not connected to that user."));
            }

            return Ok(connection);
        }

        [HttpGet]
        public async Task<IActionResult> IncomingConnections()
        {
            var targerUsername = User.Identity.Name;

            var connections = await _context.Connections.Where(connection => connection.TargetUsername == targerUsername).ToListAsync();

            return Ok(connections);
        }

        [HttpPost]
        public async Task<IActionResult> AcceptConnection([FromQuery] string username, [FromQuery] bool accept = false)
        {
            if (username == null)
            {
                return BadRequest(new ErrorDto("A username must be specified."));
            }

            var targetUsername = User.Identity.Name;

            var connection = await _context.Connections.FirstOrDefaultAsync(c => c.RequestorUsername == username && c.TargetUsername == targetUsername);

            if (connection == null)
            {
                return BadRequest(new ErrorDto("You are not connected to that user."));
            }

            connection.Accepted = accept;
            connection.Declined = !accept;

            await _context.SaveChangesAsync();

            return Ok(connection);
        }

        [HttpGet]
        public async Task<IActionResult> AcceptedConnectionInfo([FromQuery] string username)
        {
            if (username == null)
            {
                return BadRequest(new ErrorDto("A username must be specified."));
            }

            var targetUsername = User.Identity.Name;

            var connection = await _context.Connections.FirstOrDefaultAsync(c => (c.RequestorUsername == username || c.RequestorUsername == targetUsername) && (c.TargetUsername == targetUsername || c.TargetUsername == username));

            if (connection == null)
            {
                return BadRequest(new ErrorDto("You are not connected to that user."));
            }

            if (!connection.Accepted)
            {
                return BadRequest(new ErrorDto("The connection has not been accepted."));
            }

            var otherUser = await _userManager.FindByNameAsync(username);

            return Ok(new { Email = otherUser.Email });
        }
    }
}
