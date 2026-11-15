
using FootballWorld.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace FootballWorld.Data
{
    public class FootballDbContext : DbContext
    {
        public FootballDbContext(
            DbContextOptions<FootballDbContext> options)
            : base(options)
        {
        }

        public DbSet<League> Leagues { get; set; }

        public DbSet<Team> Teams { get; set; }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<League>()
                .HasMany(x => x.Teams)
                .WithOne(x => x.League)
                .HasForeignKey(x => x.LeagueId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
