using System.ComponentModel.DataAnnotations;

namespace ZenlessZoneZeroWiki.Dto
{
    public class UserDeleteDTO
    {
        [Required(ErrorMessage = "You must confirm by typing 'yes'.")]
        public string Confirmation { get; set; }
    }
}
