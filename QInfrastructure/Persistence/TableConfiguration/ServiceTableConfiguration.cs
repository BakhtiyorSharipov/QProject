using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QDomain.Models;

namespace QInfrastructure.Persistence.TableConfiguration;

public class ServiceTableConfiguration: IEntityTypeConfiguration<ServiceEntity>
{
    public void Configure(EntityTypeBuilder<ServiceEntity> builder)
    {
        builder.ToTable("Services");
        builder.HasKey(s => s.Id);

        builder.HasOne(s => s.Company)
            .WithMany(s => s.Services)
            .HasForeignKey(s => s.CompanyId);

        builder.HasMany(s => s.Employees)
            .WithOne(s => s.Service)
            .HasForeignKey(s => s.ServiceId);
    }
}