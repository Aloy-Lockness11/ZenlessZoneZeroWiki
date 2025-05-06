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

            // Set default tiers and seed prices
            SetDefaultTiers(context);
            SeedRandomPrices(context);
            SetSignatureWeapons(context);

            // Hardcode admin user if not exists
            if (!context.Users.Any(u => u.IsAdmin))
            {
                context.Users.Add(new Models.User
                {
                    FirebaseUid = "admin-firebase-uid", 
                    Username = "admin",
                    Email = "admin@zenlesszonezero.com",
                    FirstName = "Admin",
                    LastName = "User",
                    TimeCreated = DateTime.UtcNow,
                    ApiKey = null, // Or generate/set as needed
                    ApiKeyCreated = null,
                    IsAdmin = true
                });
                context.SaveChanges();
            }
        }

        public static void SeedRandomPrices(ZenlessZoneZeroContext context)
        {
            var rand = new Random();

            decimal GetPriceByTier(string tier)
            {
                return tier switch
                {
                    "S+" => Math.Round((decimal)(rand.NextDouble() * 50 + 450), 2), // 450–500
                    "S"  => Math.Round((decimal)(rand.NextDouble() * 49 + 400), 2),  // 400–449
                    "A"  => Math.Round((decimal)(rand.NextDouble() * 99 + 300), 2), // 300–399
                    "B"  => Math.Round((decimal)(rand.NextDouble() * 99 + 200), 2), // 200–299
                    "C"  => Math.Round((decimal)(rand.NextDouble() * 99 + 100), 2), // 100–199
                    "D"  => Math.Round((decimal)(rand.NextDouble() * 49 + 50), 2),  // 50–99
                    _     => Math.Round((decimal)(rand.NextDouble() * 49 + 50), 2),  // Default to D tier
                };
            }

            // Assign prices to characters based on tier
            foreach (var character in context.Characters)
            {
                character.Price = GetPriceByTier(character.Tier);
            }

            // Assign prices to weapons based on tier
            foreach (var weapon in context.Weapons)
            {
                weapon.Price = GetPriceByTier(weapon.Tier);
            }

            context.SaveChanges();
        }

        public static void SetDefaultTiers(ZenlessZoneZeroContext context)
        {
            // Set all characters to 'A' tier (edit as needed)
            foreach (var character in context.Characters)
            {
                character.Tier = "A";
            }

            // Set all weapons to 'B' tier (edit as needed)
            foreach (var weapon in context.Weapons)
            {
                weapon.Tier = "B";
            }

            context.SaveChanges();
        }

        public static void SetSignatureWeapons(ZenlessZoneZeroContext context)
        {
            var signaturePairs = new Dictionary<string, string>
            {
                { "Alexandrina Sebastiane", "Weeping Cradle" },
                { "Anby Demara", "Demara Battery Mark II" },
                { "Anton Ivanov", "Drill Rig – Red Axis" },
                { "Asaba Harumasa", "Zanshin Herb Case" },
                { "Astra Yao", "Elegant Vanity" },
                { "Ben Bigger", "Big Cylinder" },
                { "Billy Kid", "Starlight Engine Replica" },
                { "Burnice White", "Flamemaker Shaker" },
                { "Caesar King", "Tusks of Fury" },
                { "Corin Wickes", "Housekeeper" },
                { "Ellen Joe", "Deep Sea Visitor" },
                { "Evelyn Chevalier", "Heartstring Nocturne" },
                { "Grace Howard", "Fusion Compiler" },
                { "Hoshimi Miyabi", "Hailstorm Shrine" },
                { "Jane Doe", "Sharpened Stinger" },
                { "Koleda Belobog", "Hellfire Gears" },
                { "Lighter", "Blazing Laurel" },
                { "Luciana de Montefio", "Kaboom the Cannon" },
                { "Nekomiya Mana", "Steel Cushion" },
                { "Nicole Demara", "The Vault" },
                { "Piper Wheel", "Roaring Ride" },
                { "Qingyi", "Ice-Jade Teapot" },
                { "Seth Lowell", "Peacekeeper – Specialized" },
                { "Soldier 11", "The Brimstone" },
                { "Soukaku", "Bashful Demon" },
                { "Tsukishiro Yanagi", "Timeweaver" },
                { "Von Lycaon", "The Restrained" },
                { "Zhu Yuan", "Riot Suppressor Mark VI" }
            };

            foreach (var pair in signaturePairs)
            {
                var character = context.Characters.FirstOrDefault(c => c.Name == pair.Key);
                var weapon = context.Weapons.FirstOrDefault(w => w.Name == pair.Value);
                if (character != null && weapon != null)
                {
                    character.SignatureWeaponId = weapon.WeaponID;
                }
            }
            context.SaveChanges();
        }
    }
}
