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

        // Inject context here
        public AccountController(ZenlessZoneZeroContext context)
        {
            _context = context;
        }

        [HttpGet("Login")]
        public IActionResult Login()
        {
            var firebaseUser = HttpContext.Items["User"] as FirebaseToken;
            if (firebaseUser == null)
            {
                return Unauthorized(); // Token is invalid or missing
            }

            var uid = firebaseUser.Uid;
            var email = firebaseUser.Claims["email"]?.ToString();

           

            // Proceed with logged-in user
            return RedirectToAction("Index", "Home");
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDTO model)
        {
            var firebaseUser = HttpContext.Items["User"] as FirebaseToken;

            if (firebaseUser == null)
            {
                return Unauthorized("Firebase authentication required.");
            }

            var existingUser = await _context.Users.FindAsync(firebaseUser.Uid);

            if (existingUser != null)
            {
                return Conflict("User already exists.");
            }

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

            return Ok("User registered successfully.");
        }


        [HttpPut]
        [Route("UpdateUserDetails")]
        public async Task<IActionResult> UpdateUserDetails([FromBody] UserUpdateDTO updatedDetails)
        {
            var firebaseUser = HttpContext.Items["User"] as FirebaseToken;

            if (firebaseUser == null)
            {
                return Unauthorized("Firebase authentication required.");
            }

            var user = await _context.Users.FindAsync(firebaseUser.Uid);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.FirstName = updatedDetails.FirstName;
            user.LastName = updatedDetails.LastName;
            user.Username = updatedDetails.Username;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok("User details updated successfully.");
        }

        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser()
        {
            var firebaseUser = HttpContext.Items["User"] as FirebaseAdmin.Auth.FirebaseToken;

            if (firebaseUser == null)
            {
                return Unauthorized("Firebase authentication required.");
            }

            var user = await _context.Users.FindAsync(firebaseUser.Uid);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Remove associated favorites first due to FK constraints
            var favourites = _context.Favourites.Where(f => f.FirebaseUid == user.FirebaseUid);
            _context.Favourites.RemoveRange(favourites);

            // Remove user from database
            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            // Optional: Delete user from Firebase Auth as well 
            await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.DeleteUserAsync(firebaseUser.Uid);

            return Ok("User deleted successfully.");
        }


        public IActionResult ProtectedAction()
        {
            if (HttpContext.Items["User"] is not FirebaseToken firebaseUser)
                return Unauthorized();

            // Proceed knowing user is authenticated
            return View();
        }

        
    }
}
