using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QDomain.Models;

namespace QInfrastructure.Persistence.TableConfiguration;

public class EmployeeTableConfiguration : IEntityTypeConfiguration<EmployeeEntity>
{
    public void Configure(EntityTypeBuilder<EmployeeEntity> builder)
    {
        builder.ToTable("Employees");
        builder.HasKey(s => s.Id);

        builder.HasMany(s => s.Queues)
            .WithOne(s => s.Employee)
            .HasForeignKey(s => s.EmployeeId);

        builder.HasOne(s => s.Service)
            .WithMany(s => s.Employees)
            .HasForeignKey(s => s.ServiceId);
    }
}