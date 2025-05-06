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
    public class FavouritesController : BaseController
    {
        private readonly ZenlessZoneZeroContext _context;

        private string firebaseUid = "firebase-uid-123abc";

        public FavouritesController(ZenlessZoneZeroContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IActionResult> FavouriteListView()
        {
            var firebaseUid = GetFirebaseUid();
            if (firebaseUid == null)
                return Unauthorized(); 

            var characterFavs = await _context.Favourites
                .Where(f => f.FirebaseUid == firebaseUid && f.CharacterID != null)
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

        //Toggle feature for the stars thing

        
        [HttpPost]
        public async Task<IActionResult> ToggleFavourite(int? characterId, int? weaponId)
        {
            var firebaseUid = GetFirebaseUid();
            if (firebaseUid == null)
            {
                TempData["ErrorMessage"] = "You're not logged in.";
                return Redirect(Request.Headers["Referer"].ToString());
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);
            if (user != null && user.IsAdmin)
            {
                TempData["ErrorMessage"] = "Admins cannot modify favourites.";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            if (characterId == null && weaponId == null)
            {
                TempData["ErrorMessage"] = "Invalid favorite request.";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            try
            {
                Favourite existing = null;

                if (characterId != null)
                {
                    existing = await _context.Favourites.FirstOrDefaultAsync(f => f.FirebaseUid == firebaseUid && f.CharacterID == characterId);
                }
                else if (weaponId != null)
                {
                    existing = await _context.Favourites.FirstOrDefaultAsync(f => f.FirebaseUid == firebaseUid && f.WeaponID == weaponId);
                }

                if (existing != null)
                {
                    _context.Favourites.Remove(existing);
                    TempData["SuccessMessage"] = "Removed from your favourites.";
                }
                else
                {
                    Favourite fav = null;

                    if (characterId != null)
                    {
                        fav = new Favourite
                        {
                            FirebaseUid = firebaseUid,
                            CharacterID = characterId,
                            WeaponID = null, // explicitly set
                            TimeModified = DateTime.UtcNow
                        };
                    }
                    else if (weaponId != null)
                    {
                        fav = new Favourite
                        {
                            FirebaseUid = firebaseUid,
                            CharacterID = null,
                            WeaponID = weaponId,
                            TimeModified = DateTime.UtcNow
                        };
                    }

                    _context.Favourites.Add(fav);
                    TempData["SuccessMessage"] = "Added to your favourites!";
                }


                await _context.SaveChangesAsync();
            }
            catch
            {
                TempData["ErrorMessage"] = "Failed to update favourites.";
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }

        // POST: /Favourites/AddCharacter/5
        [HttpPost]
        public async Task<IActionResult> AddCharacter(int id)
        {
            var firebaseUid = GetFirebaseUid();
            if (firebaseUid == null)
                return Unauthorized();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);
            if (user != null && user.IsAdmin)
            {
                TempData["ErrorMessage"] = "Admins cannot add favourites.";
                return RedirectToAction(nameof(FavouriteListView));
            }

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
                TempData["SuccessMessage"] = "Character added to favorites.";
            }
            else
            {
                TempData["ErrorMessage"] = "Character is already in your favorites.";
            }

            return RedirectToAction(nameof(FavouriteListView));
        }

        // POST: /Favourites/RemoveCharacter/5
        [HttpPost]
        public async Task<IActionResult> RemoveCharacter(int id)
        {
            var firebaseUid = GetFirebaseUid();
            if (firebaseUid == null)
                return Unauthorized();

            var fav = await _context.Favourites
                .FirstOrDefaultAsync(f => f.FirebaseUid == firebaseUid && f.CharacterID == id);

            if (fav != null)
            {
                _context.Favourites.Remove(fav);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Character removed from favorites.";
            }
            else
            {
                TempData["ErrorMessage"] = "Character not found in your favorites.";
            }

            return RedirectToAction(nameof(FavouriteListView));
        }

        // POST: /Favourites/AddWeapon/5
        [HttpPost]
        public async Task<IActionResult> AddWeapon(int id)
        {
            var firebaseUid = GetFirebaseUid();
            if (firebaseUid == null)
                return Unauthorized();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);
            if (user != null && user.IsAdmin)
            {
                TempData["ErrorMessage"] = "Admins cannot add favourites.";
                return RedirectToAction(nameof(FavouriteListView));
            }

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
                TempData["SuccessMessage"] = "Weapon added to favorites.";
            }
            else
            {
                TempData["ErrorMessage"] = "Weapon is already in your favorites.";
            }

            return RedirectToAction(nameof(FavouriteListView));
        }

        // POST: /Favourites/RemoveWeapon/5
        [HttpPost]
        public async Task<IActionResult> RemoveWeapon(int id)
        {
            var firebaseUid = GetFirebaseUid();
            if (firebaseUid == null)
                return Unauthorized();

            var fav = await _context.Favourites
                .FirstOrDefaultAsync(f => f.FirebaseUid == firebaseUid && f.WeaponID == id);

            if (fav != null)
            {
                _context.Favourites.Remove(fav);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Weapon removed from favorites.";
            }
            else
            {
                TempData["ErrorMessage"] = "Weapon not found in your favorites.";
            }

            return RedirectToAction(nameof(FavouriteListView));
        }

        private string GetFirebaseUid()
        {
            return HttpContext.Session.GetString("FirebaseUid");
        }

    }


}
