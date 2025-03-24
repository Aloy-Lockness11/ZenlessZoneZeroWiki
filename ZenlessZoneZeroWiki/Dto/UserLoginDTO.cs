
using System.ComponentModel.DataAnnotations;

namespace ZenlessZoneZeroWiki.Dto
{
    public class UserLoginDTO
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters to 100 characters in Length.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

