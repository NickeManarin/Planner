using System.Collections.Generic;

namespace Planner.Model.Transient
{
    public class UsersResponse : GenericResponse
    {
        public IEnumerable<UserResponse> Users { get; set; }
    }
}