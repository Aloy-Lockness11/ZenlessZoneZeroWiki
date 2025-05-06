namespace ZenlessZoneZeroWiki.Dto
{
    public class ShoppingCartItemDTO
    {
        public int ShoppingCartItemId { get; set; }
        public string ItemType { get; set; } // "Character" or "Weapon"
        public int ItemId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImageUrllink { get; set; }
        public string Tier { get; set; }
    }
} 