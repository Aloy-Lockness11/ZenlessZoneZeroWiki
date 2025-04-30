using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZenlessZoneZeroWiki.Data;

namespace ZenlessZoneZeroWiki.Controllers;

[Route("[controller]/[action]")]
public class ApiController : Controller
{
    private readonly ZenlessZoneZeroContext _db;
    public ApiController(ZenlessZoneZeroContext db) => _db = db;

    // GET /Api
    [HttpGet, HttpGet("/Api")]                 
    public async Task<IActionResult> Index()
    {
        var uid = HttpContext.Session.GetString("FirebaseUid");
        var user = uid is null ? null : await _db.Users.AsNoTracking()
                                                      .FirstOrDefaultAsync(u => u.FirebaseUid == uid);

        ViewBag.ApiKeyDate = user?.ApiKeyCreated;
        return View();                           
    }
}
