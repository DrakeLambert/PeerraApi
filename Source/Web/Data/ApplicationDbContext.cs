using DrakeLambert.Peerra.WebApi.IdentityCore.Entities;
using DrakeLambert.Peerra.WebApi.Web.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DrakeLambert.Peerra.WebApi.Web.Data
{
    public class ApplicationDbContext : IdentityUserContext<User>
    {
        public DbSet<Connection> Connections { get; set; }

        public DbSet<RequestedSkill> RequestedSkills { get; set; }

        public DbSet<Skill> Skills { get; set; }

        public DbSet<SkillRequest> SkillRequest { get; set; }

        public DbSet<UserSkill> UserSkills { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Connection>().HasKey(c => new { c.TargetUsername, c.RequestorUsername });
            builder.Entity<Connection>().HasIndex(c => c.RequestorUsername);
            builder.Entity<Connection>().HasIndex(c => c.TargetUsername);

            builder.Entity<RequestedSkill>().HasKey(r => new { r.SkillName, r.SkillRequestId });
            builder.Entity<RequestedSkill>().HasIndex(r => r.SkillName);
            builder.Entity<RequestedSkill>().HasIndex(r => r.SkillRequestId);

            builder.Entity<Skill>().HasKey(s => s.Name);

            builder.Entity<SkillRequest>().HasKey(s => s.Id);
            builder.Entity<SkillRequest>().HasIndex(s => s.RequestorUsername);

            builder.Entity<UserSkill>().HasKey(u => new { Username = u.Username, u.SkillName });
            builder.Entity<UserSkill>().HasIndex(u => u.SkillName);
            builder.Entity<UserSkill>().HasIndex(u => u.Username);

            builder.Entity<RefreshToken>().HasKey(r => r.Id);
            builder.Entity<RefreshToken>().HasIndex(r => r.Username);

            base.OnModelCreating(builder);
        }
    }
}
