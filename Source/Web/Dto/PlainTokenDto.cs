using System.ComponentModel.DataAnnotations;

namespace DrakeLambert.Peerra.WebApi.Web.Dto
{
    /// <summary>
    /// Plain text access and refresh tokens.
    /// </summary>
    public class PlainTokenDto
    {
        /// <summary>
        /// Access token.
        /// </summary>
        [Required]
        public string AccessToken { get; set; }

        /// <summary>
        /// Refresh token.
        /// </summary>
        [Required]
        public string RefreshToken { get; set; }
    }
}
