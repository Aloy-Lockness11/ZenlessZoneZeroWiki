using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ZenlessZoneZeroWiki.Models
{
    public class User
    {
        [Key]
        public string FirebaseUid { get; set; }  // Firebase UID as primary key

        [Required, StringLength(100)]
        public string Username { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, StringLength(100)]
        public string FirstName { get; set; }

        [Required, StringLength(100)]
        public string LastName { get; set; }

        public DateTime TimeCreated { get; set; } = DateTime.UtcNow;

        [MaxLength(64)]
        public string? ApiKey { get; set; }
        public DateTime? ApiKeyCreated { get; set; }

        public ICollection<Favourite> Favourites { get; set; } = new List<Favourite>();  // Initialize empty list
        public ShoppingCart ShoppingCart { get; set; } // Persistent shopping cart
    }

}
