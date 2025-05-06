using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ZenlessZoneZeroWiki.Models;
using ZenlessZoneZeroWiki.Data;

namespace ZenlessZoneZeroWiki.Controllers;

public class HomeController : BaseController
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, ZenlessZoneZeroContext context) : base(context)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
