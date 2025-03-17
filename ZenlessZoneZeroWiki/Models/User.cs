using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ZenlessZoneZeroWiki.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required, StringLength(100)]
        public string Username { get; set; } = string.Empty;  // Default empty string

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        public DateTime TimeCreated { get; set; } = DateTime.UtcNow;

        public ICollection<Favourite> Favourites { get; set; } = new List<Favourite>();  // Initialize empty list
    }

}
