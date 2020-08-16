using System.Collections.Generic;

namespace Planner.Model.Transient
{
    public class EventResponse : GenericResponse
    {
        public IEnumerable<Event> Events { get; set; }
    }
}