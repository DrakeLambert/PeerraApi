using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DrakeLambert.Peerra.WebApi.Web.Data;
using DrakeLambert.Peerra.WebApi.Web.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DrakeLambert.Peerra.WebApi.Web.Controllers
{
    public class SkillController : ApiControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SkillController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> UsersWithSkills([FromQuery, Required] string skills)
        {
            var skillList = skills.Split(' ').Where(skill => skill.Length > 0).Select(skill => skill.ToLower()).ToList();
            var userSkills = new List<UserSkill>();
            var allUserSKills = await _context.UserSkills.ToListAsync();
            foreach (var skill in skillList)
            {
                userSkills.AddRange(allUserSKills.Where(userSkill => skill.Contains(userSkill.SkillName) || userSkill.SkillName.Contains(skill)));
            }
            var groups = userSkills
                .GroupBy(userSkill => userSkill.Username)
                .OrderByDescending(userSkillGroup => userSkillGroup.Count())
                .ToDictionary(userSkill => userSkill.Key, userSkill => userSkill.ToList())
                .Select(group => new { Username = group.Key, Skills = group.Value.Select(us => us.SkillName).Distinct() });
            return Ok(groups);
        }
    }
}
