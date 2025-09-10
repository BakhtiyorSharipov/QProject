using Microsoft.EntityFrameworkCore;
using QApplication.Interfaces.Repository;
using QDomain.Models;
using QInfrastructure.Persistence.DataBase;

namespace QInfrastructure.Persistence.Repositories;

public class ServiceRepository: Repository<ServiceEntity>, IServiceRepository
{
    private readonly DbSet<ServiceEntity> _dbService;
    private readonly EFContext _context;
    
    public ServiceRepository(EFContext context) : base(context)
    {
        _dbService = context.Set<ServiceEntity>();
        _context = context;
    }
}