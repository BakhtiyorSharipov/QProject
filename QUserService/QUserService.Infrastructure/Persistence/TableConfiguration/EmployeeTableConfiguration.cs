using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QUserService.Domain.Models;

namespace QUserService.Infrastructure.Persistence.TableConfiguration;

public class EmployeeTableConfiguration : IEntityTypeConfiguration<EmployeeEntity>
{
    public void Configure(EntityTypeBuilder<EmployeeEntity> builder)
    {
        builder.ToTable("Employees");
        builder.HasKey(s => s.Id);
        builder.HasIndex(s => s.ServiceId);
        builder.HasIndex(s => s.CompanyId);
        builder.HasIndex(s => s.BranchId);

        builder.Property(s => s.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.LastName)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(s => s.Position)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(s => s.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.HasIndex(s => s.PhoneNumber)
            .IsUnique();

        


    }
}