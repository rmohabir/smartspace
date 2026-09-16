using Microsoft.EntityFrameworkCore;
using SmartSpace.Api.Domain;

namespace SmartSpace.Api.Data;

public static class DevelopmentDataSeeder
{
    public static async Task SeedAsync(SmartSpaceDbContext db, CancellationToken cancellationToken = default)
    {
        var locations = await db.Locations
            .ToDictionaryAsync(location => location.Name, cancellationToken);

        if (!locations.TryGetValue("BIDN Hoofdkantoor", out var headquarters))
        {
            headquarters = new Location
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                Name = "BIDN Hoofdkantoor",
                Building = "A",
                Floor = "2"
            };
            db.Locations.Add(headquarters);
        }

        if (!locations.TryGetValue("BIDN Studio", out var studio))
        {
            studio = new Location
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                Name = "BIDN Studio",
                Building = "B",
                Floor = "1"
            };
            db.Locations.Add(studio);
        }

        var resourceNames = await db.Resources
            .Select(resource => resource.Name)
            .ToHashSetAsync(cancellationToken);
        var demoResources = new[]
        {
            new Resource { Id = Guid.Parse("20000000-0000-0000-0000-000000000001"), Name = "Orchidee", Capacity = 8, Location = headquarters },
            new Resource { Id = Guid.Parse("20000000-0000-0000-0000-000000000002"), Name = "Berk", Capacity = 12, Location = headquarters },
            new Resource { Id = Guid.Parse("20000000-0000-0000-0000-000000000003"), Name = "Duin", Capacity = 6, Location = studio }
        };

        db.Resources.AddRange(demoResources.Where(resource => !resourceNames.Contains(resource.Name)));

        await db.SaveChangesAsync(cancellationToken);
    }
}
