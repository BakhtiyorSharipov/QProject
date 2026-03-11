using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QBranchService.Domain.Models;

namespace QBranchService.Infrastructure.Persistence.TableConfiguration;

public class CompanyServiceTableConfiguration: IEntityTypeConfiguration<CompanyServiceEntity>
{
    public void Configure(EntityTypeBuilder<CompanyServiceEntity> builder)
    {
        builder.ToTable("Services");
        builder.HasKey(s => s.Id);

        builder.HasOne(s => s.Company)
            .WithMany(s => s.CompanyServices)
            .HasForeignKey(s => s.CompanyId);

        // builder.HasMany(s => s.Employees)
        //     .WithOne(s => s.Service)
        //     .HasForeignKey(s => s.ServiceId);
    }
}