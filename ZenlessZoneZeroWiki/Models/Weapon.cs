using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ZenlessZoneZeroWiki.Models
{
    public class Weapon
    {
        [Key]
        public int WeaponID { get; set; }
        public string Name { get; set; }
        public int AttackDMG { get; set; }
        public int Defence { get; set; }

        public WeaponType Type { get; set; }
       
        public string Description { get; set; }
        public string ImageUrllink { get; set; }

        public ICollection<Favourite> Favourites { get; set; } = new List<Favourite>();
        public decimal Price { get; set; } // Euro price
        public string Tier { get; set; } // Meta tier (S+, S, A, B, C, D)
    }
}
