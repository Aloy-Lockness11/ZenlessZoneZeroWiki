using System;
using System.ComponentModel.DataAnnotations;

namespace ZenlessZoneZeroWiki.Models
{
    public class PurchaseHistory
    {
        [Key]
        public int PurchaseHistoryId { get; set; }
        public string UserFirebaseUid { get; set; }
        public string ItemType { get; set; } // "Character" or "Weapon"
        public int ItemId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
        public string ImageUrllink { get; set; }
        public string Tier { get; set; }
    }
} 