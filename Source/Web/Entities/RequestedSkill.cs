using System;

namespace DrakeLambert.Peerra.WebApi.Web.Entities
{
    public class RequestedSkill
    {
        public string SkillName { get; set; }

        public Guid SkillRequestId { get; set; }
    }
}
