using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QUserService.Domain.Models;

namespace QUserService.Infrastructure.Persistence.TableConfiguration;

public class BlockedCustomerTableConfiguration: IEntityTypeConfiguration<BlockedCustomerEntity>
{
    public void Configure(EntityTypeBuilder<BlockedCustomerEntity> builder)
    {
        builder.ToTable("BlockedCustomers");
        builder.HasKey(s => s.Id);
        builder.HasIndex(s => s.CompanyId);

        builder.HasOne(s => s.Customer)
            .WithMany()
            .HasForeignKey(s => s.CustomerId);
    }
}