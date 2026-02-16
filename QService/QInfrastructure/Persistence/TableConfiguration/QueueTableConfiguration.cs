using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QDomain.Models;

namespace QInfrastructure.Persistence.TableConfiguration;

public class QueueTableConfiguration: IEntityTypeConfiguration<QueueEntity>
{
    public void Configure(EntityTypeBuilder<QueueEntity> builder)
    {
        builder.ToTable("Queues");
        builder.HasKey(s => s.Id);
        
        builder.HasOne(s => s.Customer)
            .WithMany(s => s.Queues)
            .HasForeignKey(s => s.CustomerId);

        builder.HasOne(s => s.Employee)
            .WithMany(s => s.Queues)
            .HasForeignKey(s => s.EmployeeId);

        builder.HasOne(s => s.Service)
            .WithMany()
            .HasForeignKey(s => s.ServiceId);

    }
}