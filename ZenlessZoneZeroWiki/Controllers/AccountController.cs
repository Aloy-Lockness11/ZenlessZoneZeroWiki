using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZenlessZoneZeroWiki.Data;
using ZenlessZoneZeroWiki.Dto;
using ZenlessZoneZeroWiki.Models;       

namespace ZenlessZoneZeroWiki.Controllers;

[Route("[controller]/[action]")]
public class AccountController : Controller
{
    private readonly ZenlessZoneZeroContext _db;
    public AccountController(ZenlessZoneZeroContext db) => _db = db;

    /*──────────────────────── LOGIN ───────────────────────*/

    [HttpGet("Login")]
    public IActionResult Login(string? message = null)
    {
        if (!string.IsNullOrEmpty(message))
            TempData["ErrorMessage"] = message;
        return View();
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromForm] UserLoginDTO m)
    {
        if (!ModelState.IsValid) return View("Login", m);

        try
        {
            var rec = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(m.Email);
            var tok = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(rec.Uid);

            if (await _db.Users.FirstOrDefaultAsync(u => u.Email == m.Email) is null)
            {
                TempData["ErrorMessage"] = "User not registered.";
                return View("Login", m);
            }

            HttpContext.Session.SetString("AuthToken", tok);
            HttpContext.Session.SetString("FirebaseUid", rec.Uid);
            return RedirectToAction("Index", "Home");
        }
        catch (FirebaseAuthException)
        {
            TempData["ErrorMessage"] = "Invalid email or password.";
            return View("Login", m);
        }
    }

    [HttpPost("Logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        TempData["SuccessMessage"] = "You’ve been logged out.";
        return RedirectToAction(nameof(Login));
    }

    /*───────────────── REGISTRATION (+ first API-key) ───────────────*/

    [HttpGet("Registeration")]
    public IActionResult Registeration() => View();

    [HttpPost("Registeration")]
    public async Task<IActionResult> Registeration([FromForm] UserRegistrationDTO m)
    {
        if (!ModelState.IsValid) return View(m);

        try
        {
            var fb = await FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs
            {
                Email = m.Email,
                Password = m.Password,
                DisplayName = m.Username
            });

            // mint first key
            var (plain, hashed) = ApiKeyUtil.NewKey();

            var user = new User
            {
                FirebaseUid = fb.Uid,
                Username = m.Username,
                Email = m.Email,
                FirstName = m.FirstName,
                LastName = m.LastName,
                TimeCreated = DateTime.UtcNow,
                ApiKey = hashed,
                ApiKeyCreated = DateTime.UtcNow
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            TempData["Key"] = plain;             
            return RedirectToAction("Index", "Api");
        }
        catch (FirebaseAuthException ex)
        {
            TempData["ErrorMessage"] = $"Firebase error: {ex.Message}";
            return View(m);
        }
    }

    /*───────────── RE-GENERATE KEY (same page) ─────────────*/

    [HttpPost("RegenerateKey")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegenerateKey()
    {
        var uid = HttpContext.Session.GetString("FirebaseUid");
        if (uid is null) return Unauthorized();

        var user = await _db.Users.FindAsync(uid);
        if (user is null) return Unauthorized();

        var (plain, hashed) = ApiKeyUtil.NewKey();
        user.ApiKey = hashed;
        user.ApiKeyCreated = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        TempData["Key"] = plain;                
        return RedirectToAction("Index", "Api"); 
    }

    /*───────────── ONE-TIME KEY PAGE (fallback) ───────────*/

    [HttpGet("ShowKey")]
    public IActionResult ShowKey()
    {
        var key = TempData["Key"] as string;
        return View("ShowKey", key);
    }

    /*──────────────────────── PROFILE ─────────────────────*/

    [HttpGet("UpdateUserForm")]
    public async Task<IActionResult> UpdateUserForm()
    {
        var uid = HttpContext.Session.GetString("FirebaseUid");
        if (uid is null) return Unauthorized();

        var u = await _db.Users.FirstOrDefaultAsync(x => x.FirebaseUid == uid);
        if (u is null) return NotFound();

        return View(new UserUpdateDetailsDTO
        {
            FirebaseUid = u.FirebaseUid,
            Email = u.Email,
            Username = u.Username,
            FirstName = u.FirstName,
            LastName = u.LastName,
            TimeCreated = u.TimeCreated
        });
    }

    [HttpPost("UpdateUser")]
    public async Task<IActionResult> UpdateUser([FromForm] UserUpdateDTO m)
    {
        if (!ModelState.IsValid) return RedirectToAction(nameof(UpdateUserForm));

        var u = await _db.Users.FirstOrDefaultAsync(x => x.FirebaseUid == m.FirebaseUid);
        if (u is null) return RedirectToAction(nameof(UpdateUserForm));

        u.Username = m.Username;
        u.FirstName = m.FirstName;
        u.LastName = m.LastName;
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = "User updated.";
        return RedirectToAction(nameof(UpdateUserForm));
    }

    /*─────────────────── DELETE ACCOUNT ──────────────────*/

    [HttpGet("DeleteUserForm")]
    public IActionResult DeleteUserForm() => View(new UserDeleteDTO());

    [HttpPost("DeleteUser")]
    public async Task<IActionResult> DeleteUser([FromForm] UserDeleteDTO m)
    {
        if (m.Confirmation?.Trim().ToLower() != "yes")
        {
            TempData["ErrorMessage"] = "Type 'yes' to confirm.";
            return RedirectToAction(nameof(DeleteUserForm));
        }

        var uid = HttpContext.Session.GetString("FirebaseUid");
        var u = await _db.Users.FirstOrDefaultAsync(x => x.FirebaseUid == uid);
        if (u is null) return Unauthorized();

        _db.Favourites.RemoveRange(_db.Favourites.Where(f => f.FirebaseUid == uid));
        _db.Users.Remove(u);
        await _db.SaveChangesAsync();

        await FirebaseAuth.DefaultInstance.DeleteUserAsync(uid);
        HttpContext.Session.Clear();

        TempData["SuccessMessage"] = "Account deleted.";
        return RedirectToAction(nameof(Login));
    }
}
