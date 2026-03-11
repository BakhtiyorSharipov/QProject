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
        builder.HasIndex(s => s.ServiceId);
        builder.HasIndex(s => s.CompanyId);
        builder.HasIndex(s => s.BranchId);

        builder.HasMany(s => s.Queues)
            .WithOne(s => s.Employee)
            .HasForeignKey(s => s.EmployeeId);

        
    }
}