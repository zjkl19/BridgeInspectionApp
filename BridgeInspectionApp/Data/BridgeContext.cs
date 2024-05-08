using Microsoft.EntityFrameworkCore;
using BridgeInspectionApp.Models;
namespace BridgeInspectionApp.Data;

public class BridgeContext : DbContext
{
    public DbSet<Bridge> Bridges { get; set; }
    public DbSet<Defect> Defects { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public BridgeContext()
    {
    }
    public BridgeContext(DbContextOptions<BridgeContext> options) : base(options)
    {
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
       
        string databasePath = Path.Combine(FileSystem.AppDataDirectory, "BridgeDatabase.db");
        optionsBuilder.UseSqlite($"Filename={databasePath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bridge>()
            .HasKey(b => b.Id);

        modelBuilder.Entity<Defect>()
            .HasKey(d => d.Id);

        modelBuilder.Entity<Photo>()
            .HasKey(p => p.Id);

        modelBuilder.Entity<Bridge>()
            .HasMany(b => b.Defects)
            .WithOne()
            .HasForeignKey(d => d.BridgeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Defect>()
            .HasMany(d => d.Photos)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
    }
}