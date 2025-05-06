using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using ZenlessZoneZeroWiki.Data;
using ZenlessZoneZeroWiki.Dto;
using ZenlessZoneZeroWiki.Models;
namespace ZenlessZoneZeroWiki.Controllers;

[Route("[controller]/[action]")]
public class AccountController : BaseController
{
    private readonly ZenlessZoneZeroContext _db;
    public AccountController(ZenlessZoneZeroContext db) : base(db) => _db = db;

    /*──────────────────────── LOGIN ───────────────────────*/

    [HttpGet("Login")]
    public IActionResult Login(string? message = null)
    {
        if (!string.IsNullOrEmpty(message))
            TempData["ErrorMessage"] = message;
        return View();
    }

    [HttpPost("GoogleLogin")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDTO model)
    {
        try
        {
            Console.WriteLine("GoogleLogin endpoint hit");
            // 1. Verify the Firebase ID token
            var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(model.IdToken);
            var uid = decodedToken.Uid;
            var email = decodedToken.Claims["email"]?.ToString();
            var name = decodedToken.Claims["name"]?.ToString();
            var firstName = name?.Split(' ')[0] ?? "Unknown";
            var lastName = name?.Split(' ').Length > 1 ? name.Split(' ')[1] : "User";

            // 2. Check if user exists in the database, create if not
            var user = await _db.Users.FirstOrDefaultAsync(u => u.FirebaseUid == uid);

            var verificationLink = await FirebaseAuth.DefaultInstance.GenerateEmailVerificationLinkAsync(email);

            // Send it using your email service
            await SendEmailAsync(email, "Verify your email",
                $"Please verify your email by clicking the following link: <a href='{verificationLink}'>Verify Email</a>");


            // mint first key
            var (plain, hashed) = ApiKeyUtil.NewKey();



            if (user == null)
            {
                // Create new user
                user = new User
                {
                    FirebaseUid = uid,
                    Username = email.Split('@')[0], // Generate username from email
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    TimeCreated = DateTime.UtcNow,
                    ApiKey = hashed,
                    ApiKeyCreated = DateTime.UtcNow
                };
                _db.Users.Add(user);

                // Create a shopping cart for the new user
                var shoppingCart = new ShoppingCart
                {
                    UserFirebaseUid = uid
                };
                _db.ShoppingCarts.Add(shoppingCart);

                await _db.SaveChangesAsync();
            }

            // 3. Set session (same as your email/password login)
            var token = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(uid);
            HttpContext.Session.SetString("AuthToken", token);
            HttpContext.Session.SetString("FirebaseUid", uid);

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, error = ex.Message });
        }
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromForm] UserLoginDTO m)
    {
        if (!ModelState.IsValid) return View("Login", m);

        try
        {
            // 1. Get Firebase user by email
            var rec = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(m.Email);

            // 2. Check if email is verified
            if (!rec.EmailVerified)
            {
                TempData["ErrorMessage"] = "Please verify your email before logging in.";
                TempData["UnverifiedEmail"] = m.Email;
                return View("Login", m);
            }


            // 3. Ensure user exists in your DB
            if (await _db.Users.FirstOrDefaultAsync(u => u.Email == m.Email) is null)
            {
                TempData["ErrorMessage"] = "User not registered.";
                return View("Login", m);
            }

            // 4. Create custom Firebase token and set session
            var tok = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(rec.Uid);
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
        TempData["SuccessMessage"] = "You've been logged out.";
        return RedirectToAction(nameof(Login));
    }
    [HttpPost("ResendVerification")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResendVerification(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            TempData["ErrorMessage"] = "Email required.";
            return RedirectToAction("Login");
        }

        try
        {
            var user = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);

            if (user.EmailVerified)
            {
                TempData["SuccessMessage"] = "Email is already verified. You can log in.";
            }
            else
            {
                var link = await FirebaseAuth.DefaultInstance.GenerateEmailVerificationLinkAsync(email);

                await SendEmailAsync(
                    email,
                    "Resend: Verify your email",
                    $@"
                <p>Hello,</p>
                <p>Click the link below to verify your email address:</p>
                <p><a href='{link}'>Verify Email</a></p>"
                );

                TempData["SuccessMessage"] = "Verification email sent again!";
            }
        }
        catch (FirebaseAuthException ex)
        {
            TempData["ErrorMessage"] = $"Error: {ex.Message}";
        }

        return RedirectToAction("Login");
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

            // Generate email verification link
            var verificationLink = await FirebaseAuth.DefaultInstance.GenerateEmailVerificationLinkAsync(m.Email);

            // Send it using your email service
            await SendEmailAsync(m.Email, "Verify your email",
                $"Please verify your email by clicking the following link: <a href='{verificationLink}'>Verify Email</a>");

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
                ApiKeyCreated = DateTime.UtcNow,
                IsAdmin = m.Email.ToLower() == "admin@zenlesszonezero.com" // Set admin for specific email
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            TempData["Key"] = plain;
            TempData["RegisteredEmail"] = m.Email;
            return RedirectToAction("AwaitVerification", "Account");

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

    [HttpGet("AwaitVerification")]
    public IActionResult AwaitVerification()
    {
        if (TempData["RegisteredEmail"] is null)
            return RedirectToAction("Login");

        return View();
    }


    private async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("aloysiuspacheco2003@gmail.com", "axim wcdg nolf oovg"),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress("aloysiuspacheco2003@gmail.com"),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };

        mailMessage.To.Add(toEmail);
        await smtpClient.SendMailAsync(mailMessage);
    }
}
