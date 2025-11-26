using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QDomain.Models;

namespace QInfrastructure.Persistence.TableConfiguration;

public class ReviewTableConfiguration: IEntityTypeConfiguration<ReviewEntity>
{
    public void Configure(EntityTypeBuilder<ReviewEntity> builder)
    {
        builder.ToTable("Reviews");
        builder.HasKey(s => s.Id);

        builder.HasOne(s => s.Customer)
            .WithMany(s => s.Reviews)
            .HasForeignKey(s => s.CustomerId);

        builder.HasOne(s => s.Queue)
            .WithMany()
            .HasForeignKey(s => s.QueueId);
    }
}