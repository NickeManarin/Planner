using System;

namespace Planner.Model.Transient
{
    public class RefreshResponse : GenericResponse
    {
        public string AccessToken { get; set; }
        public DateTime AccessTokenExpiryDateUtc { get; set; }
    }
}