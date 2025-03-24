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
            modelBuilder.Entity<Favourite>()
                .HasOne(f => f.Character)
                .WithMany(c => c.Favourites)
                .HasForeignKey(f => f.CharacterID)
                .IsRequired(false);

            modelBuilder.Entity<Favourite>()
                .HasOne(f => f.Weapon)
                .WithMany(w => w.Favourites)
                .HasForeignKey(f => f.WeaponID)
                .IsRequired(false);
        }

    }
}

