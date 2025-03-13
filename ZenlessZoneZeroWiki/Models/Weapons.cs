using System.ComponentModel.DataAnnotations;

namespace ZenlessZoneZeroWiki.Models
{

    public class Weapon
    {
        [Key]
        public int WeaponID { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required]
        public int Damage { get; set; }

        [Required, StringLength(50)]
        public string WeaponType { get; set; }

        // Navigation Property
        public ICollection<Favourites> Favourites { get; set; }
    }
}