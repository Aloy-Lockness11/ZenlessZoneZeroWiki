using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ZenlessZoneZeroWiki.Models;

namespace ZenlessZoneZeroWiki.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        [HttpGet]
        [Route("Login")]
        public IActionResult Login()
        {
            return View();
        }
    }


}

