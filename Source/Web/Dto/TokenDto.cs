using DrakeLambert.Peerra.WebApi.IdentityCore.Entities;

namespace DrakeLambert.Peerra.WebApi.Web.Dto
{
    /// <summary>
    /// Access and refresh tokens.
    /// </summary>
    public class TokenDto
    {
        /// <summary>
        /// Creates an instance from the given tuple.
        /// </summary>
        public TokenDto((AccessToken, RefreshToken) tokens)
        {
            AccessToken = tokens.Item1;
            RefreshToken = tokens.Item2;
        }

        /// <summary>
        /// The access token.
        /// </summary>
        public AccessToken AccessToken { get; set; }

        /// <summary>
        /// The refresh token.
        /// </summary>
        public RefreshToken RefreshToken { get; set; }
    }
}
