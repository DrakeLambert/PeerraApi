using System;
using System.Collections.Generic;
using System.Linq;
using DrakeLambert.Peerra.WebApi.Web.Entities;
using DrakeLambert.Peerra.WebApi.Web.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace DrakeLambert.Peerra.WebApi.Web.Data
{
    public class SeedData
    {
        private readonly (string, string)[] _names = new[]
        {
            ("Anglea", "Holding"),
            ("Bula", "Teneyck"),
            ("Delcie", "Pomeroy"),
            ("Hilde", "Capo"),
            ("Lai", "Persing"),
            ("Albina", "Dorantes"),
            ("Carline", "Wachter"),
            ("Gregory", "Hooten"),
            ("Krishna", "Caughey"),
            ("Laree", "Gladden"),
            ("Mica", "Maclean"),
            ("Gabriela", "Bartlett"),
            ("Jeanelle", "Ellman"),
            ("Horacio", "Penning"),
            ("Nikia", "Mcglasson"),
            ("Shaquita", "Trevizo"),
            ("Erlene", "Abel"),
            ("Arlean", "Chin"),
            ("Kecia", "Petrik"),
            ("Xavier", "Townson")
        };

        private readonly string _bio = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.";

        private readonly IEnumerable<string> _skills = (new[] { "Accounting", "Actuarial", "Administration", "Advertising", "Agriculture", "Animal", "Anthropology", "Applied", "Archaeology", "Architectural", "Architecture", "Art", "Audiology", "Biobehavioral", "Biochemistry", "Bioengineering", "Biological", "Biology", "Biophysics", "Bioscience", "Biotechnology", "Business", "Chemical", "Chemistry", "Children", "Civil", "Communication", "Computer", "Correction", "Crime", "Dance", "Design", "Development", "Disorder", "Earth", "Economics", "Education", "Electrical", "Elementary", "Enforcement", "Engineering", "English", "Environmental", "Estate", "Family", "Film", "Finance", "Fishery", "Food", "Forest", "General", "Geography", "Geosciences", "Graphic", "Health", "History", "Horticulture", "Hotel", "Human", "Individual", "Industrial", "Information", "Institutional", "Journalism", "Justice", "Kindergarten", "Kinesiology", "Landscape", "Languages", "Law", "Logistics", "Management", "Marine", "Marketing", "Mathematics", "Mechanical", "Media", "Meteorology", "Microbiology", "Mineral", "Modern", "Music", "Nuclear", "Nursing", "Nutrition", "Parks", "Pathology", "Philosophy", "Photography", "Physical", "Physics", "Physiology", "Policy", "Political", "Premedicine", "Psychology", "Public", "Real", "Recreation", "Rehabilitation", "Relations", "Religious", "Resource", "Restaurant", "Science", "Sciences", "Secondary", "Services", "Social", "Sociology", "Special", "Speech", "Statistics", "Studies", "Studio", "Systems", "Technology", "Telecommunications", "Theater", "Video", "Wildlife", "Women's", "Work" }).Select(skill => skill.ToLower());

        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<SeedData> _logger;

        public SeedData(ApplicationDbContext context, UserManager<User> userManager, ILogger<SeedData> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public void Seed()
        {
            var newUsers = _names.Select(name => new User
            {
                UserName = name.Item1 + name.Item2.Substring(0, 1),
                Email = name.Item1 + name.Item2 + "@email.net",
                Bio = _bio
            });

            foreach (var user in newUsers)
            {
                _userManager.CreateAsync(user, "password").GetAwaiter().GetResult();
            }

            _context.Skills.AddRange(_skills.Select(skill => new Skill(skill)));
            _context.SaveChanges();

            var userSkills = new List<UserSkill>();
            var random = new Random();
            var skillCount = _context.Skills.Count();
            var users = _context.Users.ToList();
            foreach (var user in users)
            {
                var userSkillCount = random.Next(5, 20);
                var usedSkills = new List<Skill>();
                for (var i = 0; i < userSkillCount; i++)
                {
                    Skill nextSkill = null;
                    do
                    {
                        nextSkill = _context.Skills.OrderBy(skill => skill.Name).Skip(random.Next(0, skillCount - 1)).First();
                    } while (usedSkills.Contains(nextSkill));
                    usedSkills.Add(nextSkill);
                    _context.UserSkills.Add(new UserSkill { SkillName = nextSkill.Name, Username = user.UserName });
                }
            }
            _context.SaveChanges();
            _logger.LogInformation("Done seeding data.");
        }
    }
}
