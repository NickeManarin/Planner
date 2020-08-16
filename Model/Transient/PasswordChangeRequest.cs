namespace Planner.Model.Transient
{
    public class PasswordChangeRequest
    {
        public string email { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}