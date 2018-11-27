using System;

namespace DrakeLambert.Peerra.WebApi.Web.Entities
{
    public class SkillRequest
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public string RequestorUsername { get; set; }
    }
}
