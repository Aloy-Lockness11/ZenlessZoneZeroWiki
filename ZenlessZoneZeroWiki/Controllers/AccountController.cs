using System.Diagnostics;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZenlessZoneZeroWiki.Data;
using ZenlessZoneZeroWiki.Models;
using ZenlessZoneZeroWiki.Dto;


namespace ZenlessZoneZeroWiki.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly ZenlessZoneZeroContext _context;

        public AccountController(ZenlessZoneZeroContext context)
        {
            _context = context;
        }

        [HttpGet("Login")]
        public IActionResult Login(string message = null)
        {
            if (!string.IsNullOrEmpty(message))
                TempData["ErrorMessage"] = message;

            return View();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm] UserLoginDTO model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please check your credentials and try again.";
                return View("Login", model);
            }

            try
            {
                var userRecord = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(model.Email);
                string customToken = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(userRecord.Uid);
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not registered.";
                    return View("Login", model);
                }

                HttpContext.Session.SetString("AuthToken", customToken);
                HttpContext.Session.SetString("FirebaseUid", userRecord.Uid);

                return RedirectToAction("Index", "Home");
            }
            catch (FirebaseAuthException)
            {
                TempData["ErrorMessage"] = "Invalid email or password.";
                return View("Login", model);
            }
        }

        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "You’ve been logged out.";
            return RedirectToAction("Login", "Account");
        }


        [HttpGet("Registeration")]
        public IActionResult Registeration()
        {
            return View();
        }

        [HttpPost("Registeration")]
        public async Task<IActionResult> Registeration([FromForm] UserRegistrationDTO model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill all required fields correctly.";
                return View(model);
            }

            try
            {
                var args = new UserRecordArgs
                {
                    Email = model.Email,
                    Password = model.Password,
                    DisplayName = model.Username,
                    EmailVerified = false,
                    Disabled = false
                };

                var firebaseUser = await FirebaseAuth.DefaultInstance.CreateUserAsync(args);

                var newUser = new User
                {
                    FirebaseUid = firebaseUser.Uid,
                    Username = model.Username,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    TimeCreated = DateTime.UtcNow
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Account created successfully. Please log in.";
                return RedirectToAction("Login", "Account");
            }
            catch (FirebaseAuthException ex)
            {
                TempData["ErrorMessage"] = $"Firebase error: {ex.Message}";
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Unexpected error: {ex.Message}";
                return View(model);
            }
        }


        [HttpGet("UpdateUserForm")]
        public async Task<IActionResult> UpdateUserForm()
        {
            var firebaseUid = HttpContext.Session.GetString("FirebaseUid");

            if (string.IsNullOrEmpty(firebaseUid))
            {
                return Unauthorized("User not authenticated.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);

            if (user == null)
            {
                Console.WriteLine("B");
                return NotFound("User not found.");
            }

            var model = new UserUpdateDetailsDTO
            {
                FirebaseUid = user.FirebaseUid,
                Email = user.Email,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                TimeCreated = user.TimeCreated
            };

            return View(model); // Make sure you have a corresponding view
        }


        [HttpPost("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromForm] UserUpdateDTO model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid data submitted.";
                return RedirectToAction("UpdateUserForm");
            }

            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == model.FirebaseUid);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction("UpdateUserForm");
                }

                user.Username = model.Username;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "User updated successfully.";
                return RedirectToAction("UpdateUserForm");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("UpdateUserForm");
            }
        }


        [HttpGet("DeleteUserForm")]
        public IActionResult DeleteUserForm()
        {
            return View(new UserDeleteDTO());
        }

        [HttpPost("DeleteUser")]
        public async Task<IActionResult> DeleteUser([FromForm] UserDeleteDTO model)
        {
            if (!ModelState.IsValid || model.Confirmation?.Trim().ToLower() != "yes")
            {
                TempData["ErrorMessage"] = "Account deletion was not confirmed. Please type 'yes' to proceed.";
                return RedirectToAction("DeleteAccountForm");
            }

            var firebaseUid = HttpContext.Session.GetString("FirebaseUid");
            if (string.IsNullOrEmpty(firebaseUid))
            {
                return Unauthorized("User not authenticated.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("DeleteAccountForm");
            }

            // Remove user-related data 
            var favourites = _context.Favourites.Where(f => f.FirebaseUid == firebaseUid);
            _context.Favourites.RemoveRange(favourites);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            await FirebaseAuth.DefaultInstance.DeleteUserAsync(firebaseUid);

            HttpContext.Session.Clear();

            TempData["SuccessMessage"] = "Your account has been deleted.";
            return RedirectToAction("Login", "Account");
        }

    }
}
