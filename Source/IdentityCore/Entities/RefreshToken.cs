using System;

namespace DrakeLambert.Peerra.WebApi.IdentityCore.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }

        public string Token { get; set; }

        public DateTimeOffset Expiration { get; set; }

        public string IpAddress { get; set; }

        public string Username { get; set; }
    }
}
