﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ZenlessZoneZeroWiki.Models
{
    public class Character
    {
        [Key]
        public int CharacterID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Enum faction { get; set; }
        public int HP { get; set; }
        public int Attack { get; set; }
        public int Defence { get; set; }

        public Element Element { get; set; }

        public ICollection<Favourite> Favourites { get; set; }
    }
}
