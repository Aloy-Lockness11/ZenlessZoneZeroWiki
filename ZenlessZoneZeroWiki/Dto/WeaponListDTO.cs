using ZenlessZoneZeroWiki.Models;

namespace ZenlessZoneZeroWiki.Dto
{
    public class WeaponListDTO
    {
        public List<Weapon> Weapons { get; set; } = new();
        public List<int> FavoritedWeaponIds { get; set; } = new();
    }
}
