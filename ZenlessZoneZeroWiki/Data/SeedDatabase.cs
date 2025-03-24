using Microsoft.EntityFrameworkCore;

namespace ZenlessZoneZeroWiki.Data
{
    public class SeedDatabase
    {
        public static void Initialize(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ZenlessZoneZeroContext>();

            // Ensure database is created
            context.Database.EnsureCreated();

            // Check if data exists
            if (!context.Characters.Any() && !context.Weapons.Any())
            {
                var charactersSql = File.ReadAllText("Data/Characters.sql");
                var weaponsSql = File.ReadAllText("Data/Weapons.sql");

                context.Database.ExecuteSqlRaw(charactersSql);
                context.Database.ExecuteSqlRaw(weaponsSql);
            }
        }
    }
}
