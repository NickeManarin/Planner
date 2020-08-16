using System.Collections.Generic;

namespace Planner.Model.Transient
{
    public class ParticipationResponse : GenericResponse
    {
        public IEnumerable<Participation> Participations { get; set; }
    }
}