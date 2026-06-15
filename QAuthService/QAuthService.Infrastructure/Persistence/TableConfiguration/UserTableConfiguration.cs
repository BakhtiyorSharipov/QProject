using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QAuthService.Domain.Models;

namespace QAuthService.Infrastructure.Persistence.TableConfiguration;

public class UserTableConfiguration: IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.EmailAddress).IsRequired().HasMaxLength(150);
        builder.HasIndex(u => u.EmailAddress).IsUnique();
        builder.HasIndex(u => u.CustomerId);
        builder.HasIndex(u => u.EmployeeId);

        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.Roles).IsRequired();
        builder.Property(u => u.CreatedAt).IsRequired();
        
        
        builder.HasMany(u => u.RefreshTokens)
            .WithOne(t => t.UserEntity)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}