using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZenlessZoneZeroWiki.Data;
using ZenlessZoneZeroWiki.Models;
using ZenlessZoneZeroWiki.DTOs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ZenlessZoneZeroWiki.Controllers
{
    public class FavouritesController : Controller
    {
        private readonly ZenlessZoneZeroContext _context;

        private string firebaseUid = "firebase-uid-123abc";

        public FavouritesController(ZenlessZoneZeroContext context)
        {
            _context = context;
        }

        // Get all favourites for the current user
        public async Task<IActionResult> FavouriteListView()
        {
            var firebaseUid = "firebase-uid-123abc";

            var characterFavs = await _context.Favourites
                .Where(f => f.FirebaseUid == "firebase-uid-123abc" && f.CharacterID != null)
                .Include(f => f.Character)
                .Select(f => new FavouriteListDTO
                {
                    CharacterID = f.CharacterID,
                    Name = f.Character.Name,
                    ImageUrl = f.Character.ImageUrllink
                })
                .ToListAsync();



            var weaponFavs = await _context.Favourites
                .Where(f => f.FirebaseUid == firebaseUid && f.WeaponID != null)
                .Include(f => f.Weapon)
                .Select(f => new FavouriteListDTO
                {
                    WeaponID = f.WeaponID,
                    Name = f.Weapon.Name,
                    ImageUrl = f.Weapon.ImageUrllink
                })
                .ToListAsync();


            var combined = characterFavs.Concat(weaponFavs).ToList();
            return View(combined);
        }

        // POST: /Favourites/AddCharacter/5
        [HttpPost]
        public async Task<IActionResult> AddCharacter(int id)
        {
            var exists = await _context.Favourites
                .AnyAsync(f => f.FirebaseUid == firebaseUid && f.CharacterID == id);

            if (!exists)
            {
                var favourite = new Favourite
                {
                    FirebaseUid = firebaseUid,
                    CharacterID = id,
                    TimeModified = DateTime.UtcNow
                };

                _context.Favourites.Add(favourite);
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true });
        }

        // POST: /Favourites/RemoveCharacter/5
        [HttpPost]
        public async Task<IActionResult> RemoveCharacter(int id)
        {
            var fav = await _context.Favourites
                .FirstOrDefaultAsync(f => f.FirebaseUid == firebaseUid && f.CharacterID == id);

            if (fav != null)
            {
                _context.Favourites.Remove(fav);
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true });
        }

        // POST: /Favourites/AddWeapon/5
        [HttpPost]
        public async Task<IActionResult> AddWeapon(int id)
        {
            var exists = await _context.Favourites
                .AnyAsync(f => f.FirebaseUid == firebaseUid && f.WeaponID == id);

            if (!exists)
            {
                var favourite = new Favourite
                {
                    FirebaseUid = firebaseUid,
                    WeaponID = id,
                    TimeModified = DateTime.UtcNow
                };

                _context.Favourites.Add(favourite);
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true });
        }

        // POST: /Favourites/RemoveWeapon/5
        [HttpPost]
        public async Task<IActionResult> RemoveWeapon(int id)
        {
            var fav = await _context.Favourites
                .FirstOrDefaultAsync(f => f.FirebaseUid == firebaseUid && f.WeaponID == id);

            if (fav != null)
            {
                _context.Favourites.Remove(fav);
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true });
        }
    }


}
