﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZenlessZoneZeroWiki.Data;
using ZenlessZoneZeroWiki.Dto;
using ZenlessZoneZeroWiki.Models;

namespace ZenlessZoneZeroWiki.Controllers
{
    public class CharactersController : BaseController
    {
        public CharactersController(ZenlessZoneZeroContext context) : base(context)
        {
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
        public IActionResult Create() => View();

        // POST: Characters/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("CharacterID,Name,Description,faction,HP,Attack,Defence,Element,AllowedWeaponType")]
            Character character)
        {
            if (!ModelState.IsValid) return View(character);

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
            var isAdmin = false;
            if (!string.IsNullOrEmpty(firebaseUid))
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);
                isAdmin = user?.IsAdmin ?? false;
            }
            ViewBag.IsAdmin = isAdmin;

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
            // populate characters
            var chars = await _context.Characters
                .Select(c => new SelectListItem
                {
                    Value = c.CharacterID.ToString(),
                    Text = c.Name
                })
                .ToListAsync();

            // populate *all* weapons
            var weps = await _context.Weapons
                .Select(w => new SelectListItem
                {
                    Value = w.WeaponID.ToString(),
                    Text = w.Name
                })
                .ToListAsync();

            var vm = new CharacterWeaponComparisonViewModel
            {
                AllCharacters = chars,
                FirstAllowedWeapons = weps,
                SecondAllowedWeapons = weps
            };

            return View(vm);
        }

        // POST: /Characters/CompareWithWeapon
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CompareWithWeapon(CharacterWeaponComparisonViewModel vm)
        {
            // repopulate dropdowns
            vm.AllCharacters = await _context.Characters
                .Select(c => new SelectListItem
                {
                    Value = c.CharacterID.ToString(),
                    Text = c.Name
                })
                .ToListAsync();

            var weps = await _context.Weapons
                .Select(w => new SelectListItem
                {
                    Value = w.WeaponID.ToString(),
                    Text = w.Name
                })
                .ToListAsync();

            vm.FirstAllowedWeapons = weps;
            vm.SecondAllowedWeapons = weps;

            // load selections
            vm.FirstCharacter = vm.FirstCharacterId is int fc ? await _context.Characters.FindAsync(fc) : null;
            vm.SecondCharacter = vm.SecondCharacterId is int sc ? await _context.Characters.FindAsync(sc) : null;
            vm.FirstWeapon = vm.FirstWeaponId is int fw ? await _context.Weapons.FindAsync(fw) : null;
            vm.SecondWeapon = vm.SecondWeaponId is int sw ? await _context.Weapons.FindAsync(sw) : null;

            // validation
            if (vm.FirstCharacter == null)
                ModelState.AddModelError(nameof(vm.FirstCharacterId), "Please pick a left character.");
            if (vm.SecondCharacter == null)
                ModelState.AddModelError(nameof(vm.SecondCharacterId), "Please pick a right character.");

            if (!ModelState.IsValid)
                return View(vm);

            // build observations + tally
            vm.LeftWins = 0;
            vm.RightWins = 0;
            vm.Observations.Clear();

            void CompareStat(string label, int left, int right)
            {
                if (left > right)
                {
                    vm.LeftWins++;
                    vm.Observations.Add($"Left side wins on {label} ({left} vs {right}).");
                }
                else if (right > left)
                {
                    vm.RightWins++;
                    vm.Observations.Add($"Right side wins on {label} ({right} vs {left}).");
                }
                else
                {
                    vm.Observations.Add($"{label} is tied at {left}.");
                }
            }

            CompareStat("Raw Attack", vm.FirstCharacter.Attack, vm.SecondCharacter.Attack);
            CompareStat("Raw Defence", vm.FirstCharacter.Defence, vm.SecondCharacter.Defence);
            CompareStat("Total Attack", vm.FirstCombinedAttack, vm.SecondCombinedAttack);
            CompareStat("Total Defence", vm.FirstCombinedDefence, vm.SecondCombinedDefence);

            return View(vm);
        }

        // JSON endpoint: filter only allowed weapons by character's AllowedWeaponType
        [HttpGet]
        public async Task<IActionResult> GetAllowedWeapons(int characterId)
        {
            var ch = await _context.Characters.FindAsync(characterId);
            if (ch == null) return NotFound();

            var list = await _context.Weapons
                .Where(w => w.Type == ch.AllowedWeaponType)
                .Select(w => new {
                    weaponID = w.WeaponID,
                    name = w.Name
                })
                .ToListAsync();

            return Json(list);
        }

        [HttpGet]
        public IActionResult AddCharacterView()
        {
            if (!ViewBag.IsAdmin)
            {
                return RedirectToAction("CharacterListView");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCharacter(Character character)
        {
            if (!ViewBag.IsAdmin)
            {
                return RedirectToAction("CharacterListView");
            }

            if (ModelState.IsValid)
            {
                _context.Add(character);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CharacterListView));
            }
            return View("AddCharacterView", character);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCharacter(int id)
        {
            if (!ViewBag.IsAdmin)
            {
                return RedirectToAction("CharacterListView");
            }
            var character = await _context.Characters.FindAsync(id);
            if (character != null)
            {
                _context.Characters.Remove(character);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("CharacterListView");
        }

        [HttpGet]
        public async Task<IActionResult> EditCharacterView(int id)
        {
            if (!ViewBag.IsAdmin)
            {
                return RedirectToAction("CharacterListView");
            }
            var character = await _context.Characters.FindAsync(id);
            if (character == null)
            {
                return RedirectToAction("CharacterListView");
            }
            return View(character);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCharacter(Character character)
        {
            if (!ViewBag.IsAdmin)
            {
                return RedirectToAction("CharacterListView");
            }
            if (ModelState.IsValid)
            {
                var existing = await _context.Characters.FindAsync(character.CharacterID);
                if (existing == null)
                {
                    return RedirectToAction("CharacterListView");
                }
                // Update only the editable fields
                existing.Name = character.Name;
                existing.Description = character.Description;
                existing.faction = character.faction;
                existing.CharacterType = character.CharacterType;
                existing.HP = character.HP;
                existing.Attack = character.Attack;
                existing.Defence = character.Defence;
                existing.Element = character.Element;
                existing.TypeUrllink = character.TypeUrllink;
                existing.ImageUrllink = character.ImageUrllink;
                existing.FactionimageUrllink = character.FactionimageUrllink;
                existing.AllowedWeaponType = character.AllowedWeaponType;
                existing.Price = character.Price;
                existing.Tier = character.Tier;
                existing.SignatureWeaponId = character.SignatureWeaponId;
                await _context.SaveChangesAsync();
                return RedirectToAction("CharacterListView");
            }
            return View("EditCharacterView", character);
        }
    }
}
