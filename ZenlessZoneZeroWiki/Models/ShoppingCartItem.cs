using System.ComponentModel.DataAnnotations;

namespace ZenlessZoneZeroWiki.Models
{
    public class ShoppingCartItem
    {
        [Key]
        public int ShoppingCartItemId { get; set; }
        public int ShoppingCartId { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
        public string ItemType { get; set; } // "Character" or "Weapon"
        public int ItemId { get; set; } // CharacterID or WeaponID
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; } = 1;
    }
} 