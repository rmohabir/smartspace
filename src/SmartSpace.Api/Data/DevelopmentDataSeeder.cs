using Microsoft.EntityFrameworkCore;
using SmartSpace.Api.Domain;

namespace SmartSpace.Api.Data;

public static class DevelopmentDataSeeder
{
    public static async Task SeedAsync(SmartSpaceDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.Locations.AnyAsync(cancellationToken))
        {
            return;
        }

        var headquarters = new Location
        {
            Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
            Name = "BIDN Hoofdkantoor",
            Building = "A",
            Floor = "2"
        };
        var studio = new Location
        {
            Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
            Name = "BIDN Studio",
            Building = "B",
            Floor = "1"
        };

        db.Locations.AddRange(headquarters, studio);
        db.Resources.AddRange(
            new Resource { Id = Guid.Parse("20000000-0000-0000-0000-000000000001"), Name = "Orchidee", Capacity = 8, Location = headquarters },
            new Resource { Id = Guid.Parse("20000000-0000-0000-0000-000000000002"), Name = "Berk", Capacity = 12, Location = headquarters },
            new Resource { Id = Guid.Parse("20000000-0000-0000-0000-000000000003"), Name = "Duin", Capacity = 6, Location = studio });

        await db.SaveChangesAsync(cancellationToken);
    }
}
