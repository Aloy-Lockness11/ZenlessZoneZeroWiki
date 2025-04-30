using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ZenlessZoneZeroWiki.Models
{
    public class Character
    {
        [Key]
        public int CharacterID { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public Faction? faction { get; set; }
        public CharacterType? CharacterType { get; set; }

        public int HP { get; set; }
        public int Attack { get; set; }
        public int Defence { get; set; }
        public Element? Element { get; set; }

        public string TypeUrllink { get; set; }
        public string ImageUrllink { get; set; }
        public string FactionimageUrllink { get; set; }

        // ← Make this the same enum as Weapon.Type:
        public WeaponType AllowedWeaponType { get; set; }

        public ICollection<Favourite> Favourites { get; set; }
    }
}
