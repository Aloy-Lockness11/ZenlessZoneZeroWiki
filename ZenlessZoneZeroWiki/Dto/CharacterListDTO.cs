using ZenlessZoneZeroWiki.Models;

namespace ZenlessZoneZeroWiki.Dto
{
    public class CharacterListDTO
    {
        public List<Character> Characters { get; set; } = new();
        public List<int> FavoritedCharacterIds { get; set; } = new();
    }
}
