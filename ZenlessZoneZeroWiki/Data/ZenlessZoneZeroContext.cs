namespace ZenlessZoneZeroWiki.Data
{
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using ZenlessZoneZeroWiki.Models;

    public class ZenlessZoneZeroContext : DbContext
    {
        public ZenlessZoneZeroContext(DbContextOptions<ZenlessZoneZeroContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<Weapon> Weapons { get; set; }
        public DbSet<Favourites> Favourites { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Composite Key Configuration for Favourites
            modelBuilder.Entity<Favourites>()
                .HasKey(f => new { f.UserID, f.CharacterID, f.WeaponID });

            base.OnModelCreating(modelBuilder);
        }
    }
}