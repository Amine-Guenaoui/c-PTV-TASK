using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using StreetService.Core.Models;

namespace StreetService.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Street> Streets { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Street>().Property(s => s.Geometry).HasColumnType("geometry");
    }
}
