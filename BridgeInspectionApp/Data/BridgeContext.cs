using Microsoft.EntityFrameworkCore;
using BridgeInspectionApp.Models;
namespace BridgeInspectionApp.Data;

public class BridgeContext : DbContext
{
    public DbSet<Bridge> Bridges { get; set; }
    public DbSet<Defect> Defects { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //optionsBuilder.UseSqlite("Filename=BridgeDatabase.db");
        string databasePath = Path.Combine(FileSystem.AppDataDirectory, "BridgeDatabase.db");
        optionsBuilder.UseSqlite($"Filename={databasePath}");
    }
}