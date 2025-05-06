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

        public ICollection<Favourite> Favourites { get; set; } = new List<Favourite>();
        public decimal Price { get; set; } // Euro price
        public string Tier { get; set; } // Meta tier (S+, S, A, B, C, D)
        public int? SignatureWeaponId { get; set; } // Signature weapon for matching set
    }
}
