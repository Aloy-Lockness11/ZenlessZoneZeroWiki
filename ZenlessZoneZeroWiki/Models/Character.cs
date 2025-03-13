using System.ComponentModel.DataAnnotations;

namespace ZenlessZoneZeroWiki.Models
{
    public enum Element
    {
        Fire, Water, Earth, Air, Electric
    }

    public enum CharacterType
    {
        Attacker, Defender, Support
    }

    public class Character
    {
        [Key]
        public int CharacterID { get; set; }

        [Required]
        public int HP { get; set; }

        [Required]
        public int AttackDMG { get; set; }

        [Required]
        public int Defence { get; set; }

        [Required]
        public Element Element { get; set; }

        [Required]
        public CharacterType Type { get; set; }

        public ICollection<Favourites> Favourites { get; set; }
    }

}