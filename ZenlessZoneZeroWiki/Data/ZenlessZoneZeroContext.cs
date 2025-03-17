using Microsoft.EntityFrameworkCore;
using ZenlessZoneZeroWiki.Models;

namespace ZenlessZoneZeroWiki.Data
{
    public class ZenlessZoneZeroContext : DbContext
    {
        public ZenlessZoneZeroContext(DbContextOptions<ZenlessZoneZeroContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<Weapon> Weapons { get; set; }
        public DbSet<Favourite> Favourites { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(u => u.FirebaseUid);

            modelBuilder.Entity<Favourite>()
                .HasKey(f => new { f.FirebaseUid, f.CharacterID, f.WeaponID });

            base.OnModelCreating(modelBuilder);
        }
    }
}
