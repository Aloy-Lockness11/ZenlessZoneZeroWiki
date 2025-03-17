using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZenlessZoneZeroWiki.Data;
using ZenlessZoneZeroWiki.Models;

namespace ZenlessZoneZeroWiki.Controllers
{
    public class FavouritesController : Controller
    {
        private readonly ZenlessZoneZeroContext _context;

        public FavouritesController(ZenlessZoneZeroContext context)
        {
            _context = context;
        }

        // GET: Favourites
        public async Task<IActionResult> Index()
        {
            var zenlessZoneZeroContext = _context.Favourites.Include(f => f.Character).Include(f => f.User).Include(f => f.Weapon);
            return View(await zenlessZoneZeroContext.ToListAsync());
        }

        // GET: Favourites/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favourite = await _context.Favourites
                .Include(f => f.Character)
                .Include(f => f.User)
                .Include(f => f.Weapon)
                .FirstOrDefaultAsync(m => m.FirebaseUid == id);
            if (favourite == null)
            {
                return NotFound();
            }

            return View(favourite);
        }

        // GET: Favourites/Create
        public IActionResult Create()
        {
            ViewData["CharacterID"] = new SelectList(_context.Characters, "CharacterID", "CharacterID");
            ViewData["FirebaseUid"] = new SelectList(_context.Users, "FirebaseUid", "FirebaseUid");
            ViewData["WeaponID"] = new SelectList(_context.Weapons, "WeaponID", "WeaponID");
            return View();
        }

        // POST: Favourites/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FavoriteID,FirebaseUid,CharacterID,WeaponID,TimeModified")] Favourite favourite)
        {
            if (ModelState.IsValid)
            {
                _context.Add(favourite);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CharacterID"] = new SelectList(_context.Characters, "CharacterID", "CharacterID", favourite.CharacterID);
            ViewData["FirebaseUid"] = new SelectList(_context.Users, "FirebaseUid", "FirebaseUid", favourite.FirebaseUid);
            ViewData["WeaponID"] = new SelectList(_context.Weapons, "WeaponID", "WeaponID", favourite.WeaponID);
            return View(favourite);
        }

        // GET: Favourites/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favourite = await _context.Favourites.FindAsync(id);
            if (favourite == null)
            {
                return NotFound();
            }
            ViewData["CharacterID"] = new SelectList(_context.Characters, "CharacterID", "CharacterID", favourite.CharacterID);
            ViewData["FirebaseUid"] = new SelectList(_context.Users, "FirebaseUid", "FirebaseUid", favourite.FirebaseUid);
            ViewData["WeaponID"] = new SelectList(_context.Weapons, "WeaponID", "WeaponID", favourite.WeaponID);
            return View(favourite);
        }

        // POST: Favourites/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("FavoriteID,FirebaseUid,CharacterID,WeaponID,TimeModified")] Favourite favourite)
        {
            if (id != favourite.FirebaseUid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(favourite);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FavouriteExists(favourite.FirebaseUid))
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
            ViewData["CharacterID"] = new SelectList(_context.Characters, "CharacterID", "CharacterID", favourite.CharacterID);
            ViewData["FirebaseUid"] = new SelectList(_context.Users, "FirebaseUid", "FirebaseUid", favourite.FirebaseUid);
            ViewData["WeaponID"] = new SelectList(_context.Weapons, "WeaponID", "WeaponID", favourite.WeaponID);
            return View(favourite);
        }

        // GET: Favourites/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favourite = await _context.Favourites
                .Include(f => f.Character)
                .Include(f => f.User)
                .Include(f => f.Weapon)
                .FirstOrDefaultAsync(m => m.FirebaseUid == id);
            if (favourite == null)
            {
                return NotFound();
            }

            return View(favourite);
        }

        // POST: Favourites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var favourite = await _context.Favourites.FindAsync(id);
            if (favourite != null)
            {
                _context.Favourites.Remove(favourite);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FavouriteExists(string id)
        {
            return _context.Favourites.Any(e => e.FirebaseUid == id);
        }
    }
}
