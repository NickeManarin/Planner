namespace Planner.Model.Transient
{
    public interface IResponse
    {
        int? Code { get; set; }
        int? Status { get; set; }
        string Message { get; set; }
    }
}