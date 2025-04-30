using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZenlessZoneZeroWiki.Data;
using ZenlessZoneZeroWiki.Dto;
using ZenlessZoneZeroWiki.Models;

namespace ZenlessZoneZeroWiki.Controllers
{
    public class WeaponsController : Controller
    {
        private readonly ZenlessZoneZeroContext _context;

        public WeaponsController(ZenlessZoneZeroContext context)
        {
            _context = context;
        }

        // GET: Weapons
        public async Task<IActionResult> Index()
        {
            return View(await _context.Weapons.ToListAsync());
        }

        // GET: Weapons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weapon = await _context.Weapons
                .FirstOrDefaultAsync(m => m.WeaponID == id);
            if (weapon == null)
            {
                return NotFound();
            }

            return View(weapon);
        }

        // GET: Weapons/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Weapons/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WeaponID,AttackDMG,Defence,Type,Description,ImageUrllink")] Weapon weapon)
        {
            if (ModelState.IsValid)
            {
                _context.Add(weapon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(weapon);
        }

        // GET: Weapons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weapon = await _context.Weapons.FindAsync(id);
            if (weapon == null)
            {
                return NotFound();
            }
            return View(weapon);
        }

        // POST: Weapons/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WeaponID,AttackDMG,Defence,Type,Description,ImageUrllink")] Weapon weapon)
        {
            if (id != weapon.WeaponID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(weapon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WeaponExists(weapon.WeaponID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(weapon);
        }

        // GET: Weapons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weapon = await _context.Weapons
                .FirstOrDefaultAsync(m => m.WeaponID == id);
            if (weapon == null)
            {
                return NotFound();
            }

            return View(weapon);
        }

        // POST: Weapons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var weapon = await _context.Weapons.FindAsync(id);
            if (weapon != null)
            {
                _context.Weapons.Remove(weapon);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Weapons/WeaponListView
        [HttpGet]
        public async Task<IActionResult> WeaponListView()
        {
            var weapons = await _context.Weapons.ToListAsync();

            var firebaseUid = HttpContext.Session.GetString("FirebaseUid");
            var favoritedWeaponIds = new List<int>();

            if (!string.IsNullOrEmpty(firebaseUid))
            {
                favoritedWeaponIds = await _context.Favourites
                    .Where(f => f.FirebaseUid == firebaseUid && f.WeaponID != null)
                    .Select(f => f.WeaponID.Value)
                    .ToListAsync();
            }

            var dto = new WeaponListDTO
            {
                Weapons = weapons,
                FavoritedWeaponIds = favoritedWeaponIds
            };

            return View("WeaponListView", dto);
        }

        // GET: /Weapons/WeaponDetailsView/5
        [HttpGet]
        public async Task<IActionResult> WeaponDetailsView(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weapon = await _context.Weapons
                .FirstOrDefaultAsync(m => m.WeaponID == id);
            if (weapon == null)
            {
                return NotFound();
            }

            return View("WeaponDetailsView", weapon);
        }

        private bool WeaponExists(int id)
        {
            return _context.Weapons.Any(e => e.WeaponID == id);
        }
    }
}
