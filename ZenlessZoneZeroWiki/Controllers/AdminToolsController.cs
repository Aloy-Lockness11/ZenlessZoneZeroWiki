using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ZenlessZoneZeroWiki.Controllers
{
    public class AdminToolsController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> VerifyAdminEmail()
        {
            var user = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync("admin@zenlesszonezero.com");
            var args = new UserRecordArgs()
            {
                Uid = user.Uid,
                EmailVerified = true
            };
            await FirebaseAuth.DefaultInstance.UpdateUserAsync(args);
            return Content("Admin email has been marked as verified!");
        }
    }
} 