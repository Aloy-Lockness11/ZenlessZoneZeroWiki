using System.ComponentModel.DataAnnotations;

namespace ZenlessZoneZeroWiki.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required, StringLength(100)]
        public string Username { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, StringLength(100)]
        public string FirstName { get; set; }

        [Required, StringLength(100)]
        public string LastName { get; set; }

        public DateTime TimeCreated { get; set; } = DateTime.UtcNow;

        public ICollection<Favourites> Favourites { get; set; }
    }
}