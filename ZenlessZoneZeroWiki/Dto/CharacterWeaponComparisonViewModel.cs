using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using ZenlessZoneZeroWiki.Models;

namespace ZenlessZoneZeroWiki.Dto
{
    public class CharacterWeaponComparisonViewModel
    {
        // ─── Dropdowns ─────────────────────────────────────
        public List<SelectListItem> AllCharacters { get; set; } = new();
        public List<SelectListItem> FirstAllowedWeapons { get; set; } = new();
        public List<SelectListItem> SecondAllowedWeapons { get; set; } = new();

        // ─── Selections ────────────────────────────────────
        public int? FirstCharacterId { get; set; }
        public int? SecondCharacterId { get; set; }
        public int? FirstWeaponId { get; set; }
        public int? SecondWeaponId { get; set; }

        // ─── Loaded Entities ───────────────────────────────
        public Character FirstCharacter { get; set; }
        public Character SecondCharacter { get; set; }
        public Weapon FirstWeapon { get; set; }
        public Weapon SecondWeapon { get; set; }

        // ─── Combined Stats ────────────────────────────────
        public int FirstCombinedAttack => (FirstCharacter?.Attack ?? 0) + (FirstWeapon?.AttackDMG ?? 0);
        public int FirstCombinedDefence => (FirstCharacter?.Defence ?? 0) + (FirstWeapon?.Defence ?? 0);
        public int SecondCombinedAttack => (SecondCharacter?.Attack ?? 0) + (SecondWeapon?.AttackDMG ?? 0);
        public int SecondCombinedDefence => (SecondCharacter?.Defence ?? 0) + (SecondWeapon?.Defence ?? 0);

        // ─── Results ───────────────────────────────────────
        public int LeftWins { get; set; }
        public int RightWins { get; set; }
        public List<string> Observations { get; set; } = new();
    }
}
