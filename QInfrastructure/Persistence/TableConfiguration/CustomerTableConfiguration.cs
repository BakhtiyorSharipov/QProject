using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QDomain.Models;

namespace QInfrastructure.Persistence.TableConfiguration;

public class CustomerTableConfiguration: IEntityTypeConfiguration<CustomerEntity>
{
    public void Configure(EntityTypeBuilder<CustomerEntity> builder)
    {
        builder.ToTable("Customers");
        builder.HasKey(s => s.Id);

        builder.HasMany(s => s.Reviews)
            .WithOne(s => s.Customer)
            .HasForeignKey(s => s.CustomerId);

        builder.HasMany(s => s.Queues)
            .WithOne(s => s.Customer)
            .HasForeignKey(s => s.CustomerId);
    }
}