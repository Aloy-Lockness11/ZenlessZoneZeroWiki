using System.ComponentModel.DataAnnotations;

namespace ZenlessZoneZeroWiki.Dto
{
    public class UserUpdateDetailsDTO
    {
        public string FirebaseUid { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime TimeCreated { get; set; } = DateTime.UtcNow;
    }
}
