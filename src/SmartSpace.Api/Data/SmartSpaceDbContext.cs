using Microsoft.EntityFrameworkCore;
using SmartSpace.Api.Domain;

namespace SmartSpace.Api.Data;

public sealed class SmartSpaceDbContext(DbContextOptions<SmartSpaceDbContext> options)
    : DbContext(options)
{
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Resource> Resources => Set<Resource>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SmartSpaceDbContext).Assembly);
    }
}
