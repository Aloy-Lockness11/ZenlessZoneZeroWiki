namespace ZenlessZoneZeroWiki.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    public class Favourites
    {
        public int UserID { get; set; }
        public int CharacterID { get; set; }
        public int WeaponID { get; set; }

        // Navigation Properties
        [ForeignKey("UserID")]
        public User User { get; set; }

        [ForeignKey("CharacterID")]
        public Character Character { get; set; }

        [ForeignKey("WeaponID")]
        public Weapon Weapon { get; set; }
    }
}
