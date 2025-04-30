using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using ZenlessZoneZeroWiki.Models;

namespace ZenlessZoneZeroWiki.Dto
{
    public class CharacterWeaponComparisonViewModel
    {
        // form inputs
        public int? FirstCharacterId { get; set; }
        public int? FirstWeaponId { get; set; }
        public int? SecondCharacterId { get; set; }
        public int? SecondWeaponId { get; set; }

        // dropdown data
        public List<SelectListItem> AllCharacters { get; set; }
        public List<SelectListItem> FirstAllowedWeapons { get; set; }
        public List<SelectListItem> SecondAllowedWeapons { get; set; }

        // loaded models
        public Character FirstCharacter { get; set; }
        public Weapon FirstWeapon { get; set; }
        public Character SecondCharacter { get; set; }
        public Weapon SecondWeapon { get; set; }

        // combined stats
        public int FirstCombinedAttack => FirstCharacter.Attack + (FirstWeapon?.AttackDMG ?? 0);
        public int FirstCombinedDefence => FirstCharacter.Defence + (FirstWeapon?.Defence ?? 0);
        public int SecondCombinedAttack => SecondCharacter.Attack + (SecondWeapon?.AttackDMG ?? 0);
        public int SecondCombinedDefence => SecondCharacter.Defence + (SecondWeapon?.Defence ?? 0);

        // pros & cons
        public List<string> Observations { get; set; } = new();
    }
}
