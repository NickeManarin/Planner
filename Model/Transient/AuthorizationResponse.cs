using System;

namespace Planner.Model.Transient
{
    /// <summary>
    /// Holds data to be sent to the client.
    /// </summary>
    public class AuthorizationResponse : GenericResponse
    {
        public string Email { get; set; }
        public string Role { get; set; }
        public string AccessToken { get; set; }
        public DateTime AccessTokenExpiryDateUtc { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryDateUtc { get; set; }
    }
}