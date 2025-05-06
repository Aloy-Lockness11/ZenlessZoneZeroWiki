using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZenlessZoneZeroWiki.Data;
using ZenlessZoneZeroWiki.Models;
using ZenlessZoneZeroWiki.Dto;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using ZenlessZoneZeroWiki.Services;

namespace ZenlessZoneZeroWiki.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ZenlessZoneZeroContext _context;
        private readonly EmailService _emailService;

        public ShoppingCartController(ZenlessZoneZeroContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: /ShoppingCart
        public async Task<IActionResult> Index()
        {
            string userId = GetCurrentUserFirebaseUid();
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }
            var cart = await _context.ShoppingCarts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserFirebaseUid == userId);

            if (cart == null)
            {
                cart = new ShoppingCart { UserFirebaseUid = userId };
                _context.ShoppingCarts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var cartDTO = new ShoppingCartDTO
            {
                ShoppingCartId = cart.ShoppingCartId,
                Items = cart.Items.Select(item =>
                {
                    string imageUrllink = null;
                    string tier = null;
                    if (item.ItemType == "Character")
                    {
                        var character = _context.Characters.FirstOrDefault(c => c.CharacterID == item.ItemId);
                        if (character != null)
                        {
                            imageUrllink = character.ImageUrllink;
                            tier = character.Tier;
                        }
                    }
                    else if (item.ItemType == "Weapon")
                    {
                        var weapon = _context.Weapons.FirstOrDefault(w => w.WeaponID == item.ItemId);
                        if (weapon != null)
                        {
                            imageUrllink = weapon.ImageUrllink;
                            tier = weapon.Tier;
                        }
                    }
                    return new ShoppingCartItemDTO
                    {
                        ShoppingCartItemId = item.ShoppingCartItemId,
                        ItemType = item.ItemType,
                        ItemId = item.ItemId,
                        Name = item.Name,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        ImageUrllink = imageUrllink,
                        Tier = tier
                    };
                }).ToList(),
                TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity)
            };

            return View(cartDTO);
        }

        // POST: /ShoppingCart/AddToCart
        [HttpPost]
        public async Task<IActionResult> AddToCart(string itemType, int itemId)
        {
            string userId = GetCurrentUserFirebaseUid();
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }
            var cart = await _context.ShoppingCarts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserFirebaseUid == userId);
            if (cart == null)
            {
                cart = new ShoppingCart { UserFirebaseUid = userId };
                _context.ShoppingCarts.Add(cart);
                await _context.SaveChangesAsync();
            }

            // Check if item already in cart
            var existingItem = cart.Items.FirstOrDefault(i => i.ItemType == itemType && i.ItemId == itemId);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                if (itemType == "Character")
                {
                    var character = await _context.Characters.FindAsync(itemId);
                    if (character != null)
                    {
                        cart.Items.Add(new ShoppingCartItem
                        {
                            ItemType = "Character",
                            ItemId = character.CharacterID,
                            Name = character.Name,
                            Price = character.Price,
                            Quantity = 1
                        });
                    }
                }
                else if (itemType == "Weapon")
                {
                    var weapon = await _context.Weapons.FindAsync(itemId);
                    if (weapon != null)
                    {
                        cart.Items.Add(new ShoppingCartItem
                        {
                            ItemType = "Weapon",
                            ItemId = weapon.WeaponID,
                            Name = weapon.Name,
                            Price = weapon.Price,
                            Quantity = 1
                        });
                    }
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: /ShoppingCart/RemoveFromCart
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int shoppingCartItemId)
        {
            string userId = GetCurrentUserFirebaseUid();
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }
            var cart = await _context.ShoppingCarts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserFirebaseUid == userId);
            if (cart != null)
            {
                var item = cart.Items.FirstOrDefault(i => i.ShoppingCartItemId == shoppingCartItemId);
                if (item != null)
                {
                    cart.Items.Remove(item);
                    _context.ShoppingCartItems.Remove(item);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            string userId = GetCurrentUserFirebaseUid();
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }
            var cart = await _context.ShoppingCarts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserFirebaseUid == userId);

            if (cart != null)
            {
                // Save purchase history
                var purchasedItems = new List<ShoppingCartItem>();
                foreach (var item in cart.Items)
                {
                    string imageUrllink = null;
                    string tier = null;
                    if (item.ItemType == "Character")
                    {
                        var character = await _context.Characters.FindAsync(item.ItemId);
                        if (character != null)
                        {
                            imageUrllink = character.ImageUrllink;
                            tier = character.Tier;
                        }
                    }
                    else if (item.ItemType == "Weapon")
                    {
                        var weapon = await _context.Weapons.FindAsync(item.ItemId);
                        if (weapon != null)
                        {
                            imageUrllink = weapon.ImageUrllink;
                            tier = weapon.Tier;
                        }
                    }

                    _context.PurchaseHistories.Add(new PurchaseHistory
                    {
                        UserFirebaseUid = userId,
                        ItemType = item.ItemType,
                        ItemId = item.ItemId,
                        Name = item.Name,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        ImageUrllink = imageUrllink,
                        Tier = tier
                    });
                    purchasedItems.Add(item);
                }

                // Send thank you email
                var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == userId);
                if (user != null)
                {
                    string emailBody = $@"<h2>Thank you for your purchase!</h2><p>Here are the items you purchased:</p><ul>";
                    foreach (var item in purchasedItems)
                    {
                        emailBody += $"<li>{item.Name} (x{item.Quantity}) - €{item.Price}</li>";
                    }
                    emailBody += "</ul><p>Your items will be delivered soon!</p>";
                    await _emailService.SendEmailAsync(user.Email, "Thank you for your purchase!", emailBody);
                }

                // Clear cart
                _context.ShoppingCartItems.RemoveRange(cart.Items);
                cart.Items.Clear();
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("PurchaseSuccessful");
        }

        public IActionResult PurchaseSuccessful()
        {
            return View();
        }

        // GET: /ShoppingCart/GetCartCount
        public async Task<IActionResult> GetCartCount()
        {
            string userId = GetCurrentUserFirebaseUid();
            if (string.IsNullOrEmpty(userId))
            {
                return Json(0);
            }

            var cart = await _context.ShoppingCarts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserFirebaseUid == userId);

            if (cart == null)
            {
                return Json(0);
            }

            int totalCount = cart.Items.Sum(item => item.Quantity);
            return Json(totalCount);
        }

        // GET: /ShoppingCart/GetPurchaseHistory
        public async Task<IActionResult> GetPurchaseHistory()
        {
            string userId = GetCurrentUserFirebaseUid();
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "User not logged in" });
            }

            var purchaseHistory = await _context.PurchaseHistories
                .Where(p => p.UserFirebaseUid == userId)
                .OrderByDescending(p => p.PurchaseDate)
                .Take(10) // Get last 10 purchases
                .ToListAsync();

            return Json(new { success = true, purchases = purchaseHistory });
        }

        // Placeholder for getting the current user's FirebaseUid
        private string GetCurrentUserFirebaseUid()
        {
            // Retrieve Firebase UID from session
            return HttpContext.Session.GetString("FirebaseUid");
        }

        [HttpPost]
        public IActionResult MarkNotificationsRead()
        {
            HttpContext.Session.SetString("NotificationsRead", "true");
            return Json(new { success = true });
        }
    }
} 