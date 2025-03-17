using Microsoft.AspNetCore.Mvc;

namespace ZenlessZoneZeroWiki.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {

        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View(); 
        }

        [HttpGet("Registration")]
        public IActionResult Registration()
        {
            return View(); 
        }

        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Items["User"] = null;
            return RedirectToAction("Login", "Account");
        }
    }
}
