using System.Linq;
using System.Threading.Tasks;
using DrakeLambert.Peerra.WebApi.Web.Data;
using DrakeLambert.Peerra.WebApi.Web.Dto;
using DrakeLambert.Peerra.WebApi.Web.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DrakeLambert.Peerra.WebApi.Web.Controllers
{
    public class UsersController : ApiControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public UsersController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> UserInfo([FromQuery] string username)
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

            var skills = await _context.UserSkills.Where(userSkill => userSkill.Username == username).Select(userSkill => userSkill.SkillName).ToListAsync();

            return Ok(new { Bio = user.Bio, Skills = skills });
        }
    }
}
