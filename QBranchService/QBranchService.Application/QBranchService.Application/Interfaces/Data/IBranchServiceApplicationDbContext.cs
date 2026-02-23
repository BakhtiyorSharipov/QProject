using Microsoft.EntityFrameworkCore;
using QBranchService.Domain.Models;

namespace QBranchService.Application.Interfaces.Data;

public interface IBranchServiceApplicationDbContext
{
    DbSet<CompanyEntity> Companies { get; set; }
    DbSet<CompanyServiceEntity> CompanyServices { get; set; }
    DbSet<BranchEntity> Branches { get; set; }
    DbSet<BranchConfigurationEntity> BranchConfigurations { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}