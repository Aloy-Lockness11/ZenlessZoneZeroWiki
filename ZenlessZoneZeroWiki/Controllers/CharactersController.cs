using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZenlessZoneZeroWiki.Data;
using ZenlessZoneZeroWiki.Models;
using ZenlessZoneZeroWiki.Dto;

namespace ZenlessZoneZeroWiki.Controllers
{
    public class CharactersController : Controller
    {
        private readonly ZenlessZoneZeroContext _context;

        public CharactersController(ZenlessZoneZeroContext context)
        {
            _context = context;
        }

        // GET: Characters
        public async Task<IActionResult> Index()
        {
            var characters = await _context.Characters.ToListAsync(); 

            var firebaseUid = HttpContext.Session.GetString("FirebaseUid");

            var favoritedCharacterIds = new List<int>();
            if (!string.IsNullOrEmpty(firebaseUid))
            {
                favoritedCharacterIds = await _context.Favourites
                    .Where(f => f.FirebaseUid == firebaseUid && f.CharacterID != null)
                    .Select(f => f.CharacterID.Value)
                    .ToListAsync();
            }

            var dto = new CharacterListDTO
            {
                Characters = characters,
                FavoritedCharacterIds = favoritedCharacterIds
            };

            return View("CharacterListView", dto); 
        }


        // GET: Characters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var character = await _context.Characters
                .FirstOrDefaultAsync(m => m.CharacterID == id);
            if (character == null) return NotFound();

            return View(character);
        }

        // GET: Characters/Create
        public IActionResult Create()
            => View();

        // POST: Characters/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("CharacterID,Name,Description,faction,HP,Attack,Defence,Element,AllowedWeaponType")]
            Character character)
        {
            if (!ModelState.IsValid)
                return View(character);

            _context.Add(character);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Characters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var character = await _context.Characters.FindAsync(id);
            if (character == null) return NotFound();

            return View(character);
        }

        // POST: Characters/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("CharacterID,Name,Description,faction,HP,Attack,Defence,Element,AllowedWeaponType")]
            Character character)
        {
            if (id != character.CharacterID) return NotFound();
            if (!ModelState.IsValid) return View(character);

            try
            {
                _context.Update(character);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CharacterExists(character.CharacterID))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Characters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var character = await _context.Characters
                .FirstOrDefaultAsync(m => m.CharacterID == id);
            if (character == null) return NotFound();

            return View(character);
        }

        // POST: Characters/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var character = await _context.Characters.FindAsync(id);
            if (character != null)
                _context.Characters.Remove(character);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Characters/CharacterListView
        [HttpGet]
        public async Task<IActionResult> CharacterListView()
        {
            var characters = await _context.Characters.ToListAsync();
            var firebaseUid = HttpContext.Session.GetString("FirebaseUid");

            var favoritedCharacterIds = new List<int>();
            if (!string.IsNullOrEmpty(firebaseUid))
            {
                favoritedCharacterIds = await _context.Favourites
                    .Where(f => f.FirebaseUid == firebaseUid && f.CharacterID != null)
                    .Select(f => f.CharacterID.Value)
                    .ToListAsync();
            }

            var dto = new CharacterListDTO
            {
                Characters = characters,
                FavoritedCharacterIds = favoritedCharacterIds
            };

            return View("CharacterListView", dto);
        }


        // GET: /Characters/CharacterDetailsView/5
        [HttpGet]
        public async Task<IActionResult> CharacterDetailsView(int? id)
        {
            if (id == null) return NotFound();

            var character = await _context.Characters
                .FirstOrDefaultAsync(m => m.CharacterID == id);
            if (character == null) return NotFound();

            return View("CharacterDetailsView", character);
        }

        private bool CharacterExists(int id)
            => _context.Characters.Any(e => e.CharacterID == id);

        // GET: /Characters/CompareWithWeapon
        [HttpGet]
        public async Task<IActionResult> CompareWithWeapon()
        {
            var chars = await _context.Characters
                .Select(c => new SelectListItem
                {
                    Value = c.CharacterID.ToString(),
                    Text = c.Name
                })
                .ToListAsync();

            var vm = new CharacterWeaponComparisonViewModel
            {
                AllCharacters = chars,
                FirstAllowedWeapons = new List<SelectListItem>(),
                SecondAllowedWeapons = new List<SelectListItem>()
            };
            return View(vm);
        }

        // POST: /Characters/CompareWithWeapon
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CompareWithWeapon(CharacterWeaponComparisonViewModel vm)
        {
            vm.AllCharacters = await _context.Characters
                .Select(c => new SelectListItem
                {
                    Value = c.CharacterID.ToString(),
                    Text = c.Name
                })
                .ToListAsync();

            vm.FirstCharacter = vm.FirstCharacterId is int fc ? await _context.Characters.FindAsync(fc) : null;
            vm.SecondCharacter = vm.SecondCharacterId is int sc ? await _context.Characters.FindAsync(sc) : null;

            if (vm.FirstCharacter == null || vm.SecondCharacter == null)
            {
                ModelState.AddModelError("", "Please select two characters.");
                vm.FirstAllowedWeapons = new();
                vm.SecondAllowedWeapons = new();
                return View(vm);
            }

            var allWeapons = await _context.Weapons.ToListAsync();

            vm.FirstAllowedWeapons = allWeapons
                .Where(w => w.Type == vm.FirstCharacter.AllowedWeaponType)
                .Select(w => new SelectListItem
                {
                    Value = w.WeaponID.ToString(),
                    Text = $"{w.Type} (+{w.AttackDMG} ATK / +{w.Defence} DEF)"
                }).ToList();

            vm.SecondAllowedWeapons = allWeapons
                .Where(w => w.Type == vm.SecondCharacter.AllowedWeaponType)
                .Select(w => new SelectListItem
                {
                    Value = w.WeaponID.ToString(),
                    Text = $"{w.Type} (+{w.AttackDMG} ATK / +{w.Defence} DEF)"
                }).ToList();

            vm.FirstWeapon = vm.FirstWeaponId is int fw ? allWeapons.FirstOrDefault(w => w.WeaponID == fw) : null;
            vm.SecondWeapon = vm.SecondWeaponId is int sw ? allWeapons.FirstOrDefault(w => w.WeaponID == sw) : null;

            if ((vm.FirstWeaponId.HasValue && vm.FirstWeapon == null) ||
                (vm.SecondWeaponId.HasValue && vm.SecondWeapon == null))
            {
                ModelState.AddModelError("", "One of the selected weapons isn’t valid for that character.");
                return View(vm);
            }

            void CompareStat(string label, int left, int right)
            {
                if (left > right)
                    vm.Observations.Add($"Left side wins on {label} ({left} vs {right}).");
                else if (right > left)
                    vm.Observations.Add($"Right side wins on {label} ({right} vs {left}).");
                else
                    vm.Observations.Add($"{label} is tied at {left}.");
            }

            CompareStat("Raw Attack", vm.FirstCharacter.Attack, vm.SecondCharacter.Attack);
            CompareStat("Raw Defence", vm.FirstCharacter.Defence, vm.SecondCharacter.Defence);
            CompareStat("Total Attack", vm.FirstCombinedAttack, vm.SecondCombinedAttack);
            CompareStat("Total Defence", vm.FirstCombinedDefence, vm.SecondCombinedDefence);

            return View(vm);
        }
    }
}
