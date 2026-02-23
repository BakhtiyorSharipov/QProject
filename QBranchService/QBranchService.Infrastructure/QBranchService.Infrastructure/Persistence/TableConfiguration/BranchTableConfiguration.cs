using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QBranchService.Domain.Models;

namespace QBranchService.Infrastructure.Persistence.TableConfiguration;

public class BranchTableConfiguration: IEntityTypeConfiguration<BranchEntity>
{
    public void Configure(EntityTypeBuilder<BranchEntity> builder)
    {
        builder.ToTable("Branches");
        builder.HasKey(s => s.Id);
        builder.HasOne(s => s.Company)
            .WithMany(s => s.Branches)
            .HasForeignKey(s => s.CompanyId);

    }
}