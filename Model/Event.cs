using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Planner.Model
{
    public class Event
    {
        public long Id { get; set; }

        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        public DateTime DueTo { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal SuggestedValue { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal SuggestedValueWithDrinks { get; set; }

        [StringLength(100)]
        public string Observation { get; set; }

        #region Many events to many...

        /// <summary>
        /// One event may have multiple participants.
        /// </summary>
        public IList<Participation> EventsUsers { get; set; }

        #endregion
    }
}