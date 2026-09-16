using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartSpace.Api.Domain;

namespace SmartSpace.Api.Data.Configurations;

public sealed class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.HasKey(reservation => reservation.Id);
        builder.Property(reservation => reservation.OwnerSubjectId).HasMaxLength(200).IsRequired();
        builder.Property(reservation => reservation.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(reservation => reservation.Version).IsConcurrencyToken();
        builder.ToTable(table => table.HasCheckConstraint(
            "CK_Reservations_EndAfterStart", "[EndUtc] > [StartUtc]"));
        builder.HasOne(reservation => reservation.Resource)
            .WithMany(resource => resource.Reservations)
            .HasForeignKey(reservation => reservation.ResourceId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(reservation => new { reservation.ResourceId, reservation.Status, reservation.StartUtc });
        builder.HasIndex(reservation => new { reservation.OwnerSubjectId, reservation.StartUtc });
    }
}
