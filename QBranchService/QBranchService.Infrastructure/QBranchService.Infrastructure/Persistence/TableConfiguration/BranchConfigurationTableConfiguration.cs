using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QBranchService.Domain.Models;

namespace QBranchService.Infrastructure.Persistence.TableConfiguration;

public class BranchConfigurationTableConfiguration: IEntityTypeConfiguration<BranchConfigurationEntity>
{
    public void Configure(EntityTypeBuilder<BranchConfigurationEntity> builder)
    {
        builder.ToTable("BranchConfigurations");
        builder.HasKey(s => s.Id);
        builder.HasOne(s => s.Branch)
            .WithMany()
            .HasForeignKey(c => c.BranchId);
    }
}