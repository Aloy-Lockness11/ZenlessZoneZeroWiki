﻿using System.Diagnostics;
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

    

        public IActionResult ProtectedAction()
        {
            if (HttpContext.Items["User"] is not FirebaseToken firebaseUser)
                return Unauthorized();

            // Proceed knowing user is authenticated
            return View();
        }
    }


}

