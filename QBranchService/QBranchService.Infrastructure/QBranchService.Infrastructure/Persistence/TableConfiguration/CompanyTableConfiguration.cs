using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QBranchService.Domain.Models;

namespace QBranchService.Infrastructure.Persistence.TableConfiguration;

public class CompanyTableConfiguration: IEntityTypeConfiguration<CompanyEntity>
{
    public void Configure(EntityTypeBuilder<CompanyEntity> builder)
    {
        builder.ToTable("Companies");
        builder.HasKey(s => s.Id);
        builder.HasMany(s => s.CompanyServices)
            .WithOne(s => s.Company)
            .HasForeignKey(s => s.CompanyId);
    }
}