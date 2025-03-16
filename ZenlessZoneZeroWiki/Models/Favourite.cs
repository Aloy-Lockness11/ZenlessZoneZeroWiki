using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZenlessZoneZeroWiki.Models
{
    public class Favourite
    {
        [Key]
        public int FavoriteID { get; set; }

        public int UserID { get; set; }
        public int CharacterID { get; set; }
        public int WeaponID { get; set; }

        public User User { get; set; }
        public Character Character { get; set; }
        public Weapon Weapon { get; set; }

        public DateTime TimeModified { get; set; }
    }
}


