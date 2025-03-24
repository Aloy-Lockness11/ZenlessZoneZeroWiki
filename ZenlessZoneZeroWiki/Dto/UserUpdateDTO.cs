using System.ComponentModel.DataAnnotations;

namespace ZenlessZoneZeroWiki.Dto
{
    public class UserUpdateDTO
    {
        [Required]
        public string FirebaseUid { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(100, ErrorMessage = "Username must be under 100 characters.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(100, ErrorMessage = "First name must be under 100 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(100, ErrorMessage = "Last name must be under 100 characters.")]
        public string LastName { get; set; }
    }
}
