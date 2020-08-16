using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Planner.Model
{
    public class User
    {
        public long Id { get; set; }

        #region Auth related properties

        [Required]
        public byte[] PasswordHash { get; set; }

        [Required]
        public byte[] PasswordSalt { get; set; }

        [Required]
        public DateTime? PasswordLastUpdatedUtc { get; set; }

        [Required]
        public string Role { get; set; }

        [Required]
        public bool WasDeactivated { get; set; }

        public DateTime? DeactivationDate { get; set; }

        #endregion

        [Required, StringLength(50, MinimumLength = 2)]
        public string Email { get; set; }

        [Required, StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }
        
        #region Many users to many...

        /// <summary>
        /// One user may participate in multiple events.
        /// </summary>
        public IList<Participation> UserEvents { get; set; }

        #endregion
    }
}