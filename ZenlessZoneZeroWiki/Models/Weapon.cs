using System.ComponentModel.DataAnnotations;

namespace ZenlessZoneZeroWiki.Models
{
    public class Weapon
    {
        [Key]
        public int WeaponID { get; set; }

        public int AttackDMG { get; set; }
        public int Defence { get; set; }

        public WeaponType Type { get; set; }

        public ICollection<Favourite> Favourites { get; set; }
    }
}
