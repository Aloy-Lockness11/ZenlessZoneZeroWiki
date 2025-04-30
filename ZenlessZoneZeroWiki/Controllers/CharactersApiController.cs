using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZenlessZoneZeroWiki.Data;

namespace ZenlessZoneZeroWiki.Controllers;

[ApiController, Route("api/characters")]
public class CharactersApiController : ControllerBase
{
    private readonly ZenlessZoneZeroContext _db;
    public CharactersApiController(ZenlessZoneZeroContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> All([FromHeader(Name = "X-Api-Key")] string key,
                                         [FromQuery] string? fields)
    {
        if (string.IsNullOrWhiteSpace(key))
            return Unauthorized(new { error = "Missing X-Api-Key" });

        var hash = Convert.ToHexString(
                       System.Security.Cryptography.SHA256.HashData(Convert.FromBase64String(key)));
        var ok = await _db.Users.AnyAsync(u => u.ApiKey == hash);
        if (!ok) return Unauthorized(new { error = "Bad API key" });

        var list = await _db.Characters.ToListAsync();
        if (string.IsNullOrWhiteSpace(fields)) return Ok(list);

        var pick = fields.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                         .Select(s => s.ToLower()).ToHashSet();

        var shaped = list.Select(c => new {
            c.CharacterID,
            Name = pick.Contains("name") ? c.Name : null,
            HP = pick.Contains("hp") ? c.HP : (int?)null,
            Attack = pick.Contains("attack") ? c.Attack : (int?)null,
            Defence = pick.Contains("defence") ? c.Defence : (int?)null,
            Element = pick.Contains("element") ? c.Element : null
        });

        return Ok(shaped);
    }
}
