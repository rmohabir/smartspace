using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartSpace.Api.Domain;

namespace SmartSpace.Api.Data.Configurations;

public sealed class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.HasKey(location => location.Id);
        builder.Property(location => location.Name).HasMaxLength(200).IsRequired();
        builder.Property(location => location.Building).HasMaxLength(100);
        builder.Property(location => location.Floor).HasMaxLength(100);
        builder.HasIndex(location => location.Name).IsUnique();
    }
}
