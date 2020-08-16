namespace Planner.Model.Transient
{
    public class RemovalRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Deactivate { get; set; }
    }
}