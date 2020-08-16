using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Planner.Model
{
    public class Participation
    {
        public long UserId { get; set; }
        public User User { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal Contribution { get; set; }

        [Required]
        public bool HasPaid { get; set; }

        [Required]
        public DateTime AddedIn { get; set; }

        public string Observation { get; set; }

        public long EventId { get; set; }
        public Event Event { get; set; }
    }
}