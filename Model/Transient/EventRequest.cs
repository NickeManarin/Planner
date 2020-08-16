using System;
using System.Collections.Generic;

namespace Planner.Model.Transient
{
    public class EventRequest
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public DateTime DueTo { get; set; }
        
        public decimal SuggestedValue { get; set; }
        
        public decimal SuggestedValueWithDrinks { get; set; }
        
        public string Observation { get; set; }

        public List<ParticipationRequest> EventsUsers { get; set; }  
    }
}