using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ZenlessZoneZeroWiki.Data;
using System.Linq;

namespace ZenlessZoneZeroWiki.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ZenlessZoneZeroContext _context;

        public BaseController(ZenlessZoneZeroContext context)
        {
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var firebaseUid = HttpContext.Session.GetString("FirebaseUid");
            var isAdmin = false;
            if (!string.IsNullOrEmpty(firebaseUid))
            {
                var user = _context.Users.FirstOrDefault(u => u.FirebaseUid == firebaseUid);
                isAdmin = user?.IsAdmin ?? false;
            }
            ViewBag.IsAdmin = isAdmin;
            base.OnActionExecuting(context);
        }
    }
} 