using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartSpace.Api.Domain;

namespace SmartSpace.Api.Data.Configurations;

public sealed class ResourceConfiguration : IEntityTypeConfiguration<Resource>
{
    public void Configure(EntityTypeBuilder<Resource> builder)
    {
        builder.HasKey(resource => resource.Id);
        builder.Property(resource => resource.Name).HasMaxLength(200).IsRequired();
        builder.Property(resource => resource.ResourceType).HasConversion<string>().HasMaxLength(50);
        builder.Property(resource => resource.Version).IsConcurrencyToken();
        builder.ToTable(table => table.HasCheckConstraint(
            "CK_Resources_Capacity_Positive", "[Capacity] >= 1"));
        builder.HasOne(resource => resource.Location)
            .WithMany(location => location.Resources)
            .HasForeignKey(resource => resource.LocationId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(resource => new { resource.LocationId, resource.Name }).IsUnique();
    }
}
