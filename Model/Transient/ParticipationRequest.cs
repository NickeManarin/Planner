using System;

namespace Planner.Model.Transient
{
    public class ParticipationRequest
    {
        public long UserId { get; set; }
        
        public long EventId { get; set; }
        
        public decimal Contribution { get; set; }

        public bool HasPaid { get; set; }

        public DateTime AddedIn { get; set; }
        
        public string Observation { get; set; }
    }
}