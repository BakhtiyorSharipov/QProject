using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QDomain.Models;


namespace QInfrastructure.Persistence.TableConfiguration;

public class AvailabilityScheduleTableConfiguration : IEntityTypeConfiguration<AvailabilityScheduleEntity>
{
    public void Configure(EntityTypeBuilder<AvailabilityScheduleEntity> builder)
    {
        builder.ToTable("AvailabilitySchedules");
        builder.HasKey(s => s.Id);

        builder.HasOne(s => s.Employee)
            .WithMany(s => s.AvailabilitySchedules)
            .HasForeignKey(s => s.EmployeeId);

        builder.Property(a => a.AvailableSlots)
            .HasColumnType("jsonb")
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<Interval<DateTimeOffset>>>(v,
                    (System.Text.Json.JsonSerializerOptions)null)
            );
    }
}