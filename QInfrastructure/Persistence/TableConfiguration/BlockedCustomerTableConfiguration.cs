using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QDomain.Models;

namespace QInfrastructure.Persistence.TableConfiguration;

public class BlockedCustomerTableConfiguration: IEntityTypeConfiguration<BlockedCustomerEntity>
{
    public void Configure(EntityTypeBuilder<BlockedCustomerEntity> builder)
    {
        builder.ToTable("BlockedCustomers");
        builder.HasKey(s => s.Id);

        builder.HasOne(s => s.Company)
            .WithMany()
            .HasForeignKey(s => s.CompanyId);

        builder.HasOne(s => s.Customer)
            .WithMany()
            .HasForeignKey(s => s.CustomerId);
    }
}