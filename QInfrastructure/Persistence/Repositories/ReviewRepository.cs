using Microsoft.EntityFrameworkCore;
using QApplication.Interfaces.Repository;
using QDomain.Models;
using QInfrastructure.Persistence.DataBase;

namespace QInfrastructure.Persistence.Repositories;

public class ReviewRepository: Repository<BlockedCustomerEntity>, IBlockedCustomerRepository
{
    private readonly DbSet<BlockedCustomerEntity> _dbBlockedCustomer;
    private readonly EFContext _context;
    
    public ReviewRepository(EFContext context) : base(context)
    {
        _dbBlockedCustomer = context.Set<BlockedCustomerEntity>();
        _context = context;
    }
}