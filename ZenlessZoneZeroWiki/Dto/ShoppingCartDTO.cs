using System.Collections.Generic;

namespace ZenlessZoneZeroWiki.Dto
{
    public class ShoppingCartDTO
    {
        public int ShoppingCartId { get; set; }
        public List<ShoppingCartItemDTO> Items { get; set; }
        public decimal TotalPrice { get; set; }
    }
} 