using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QUserService.Domain.Models;

namespace QUserService.Infrastructure.Persistence.TableConfiguration;

public class UserTableConfiguration:IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.EmailAddress).IsRequired().HasMaxLength(150);
        builder.HasIndex(u => u.EmailAddress).IsUnique();
        

        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.Roles).IsRequired();
        builder.Property(u => u.CreatedAt).IsRequired();

        builder.HasOne(u => u.Customer)
            .WithOne()
            .HasForeignKey<UserEntity>(u => u.CustomerId);

        builder.HasOne(u => u.Employee)
            .WithOne()
            .HasForeignKey<UserEntity>(u => u.EmployeeId);
        
        builder.HasMany(u => u.RefreshTokens)
            .WithOne(t => t.UserEntity)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}