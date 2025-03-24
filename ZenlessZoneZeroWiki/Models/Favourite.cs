using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZenlessZoneZeroWiki.Models
{
    public class Favourite
    {
        [Key]
        public int FavouriteID { get; set; }

        [Required]
        public string FirebaseUid { get; set; }

        public int? CharacterID { get; set; }

        [ForeignKey("CharacterID")]
        public Character? Character { get; set; }

        public int? WeaponID { get; set; }
        [ForeignKey("WeaponID")]
        public Weapon? Weapon { get; set; }

        public DateTime TimeModified { get; set; }

        // Optional user relationship
        [ForeignKey("FirebaseUid")]
        public User User { get; set; }
    }

}


