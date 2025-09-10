using Microsoft.EntityFrameworkCore;
using QApplication.Interfaces.Repository;
using QDomain.Models;
using QInfrastructure.Persistence.DataBase;

namespace QInfrastructure.Persistence.Repositories;

public class EmployeeRepository: Repository<EmployeeEntity>, IEmployeeRepository
{
    private readonly DbSet<EmployeeEntity> _dbEmployee;
    private readonly EFContext _context;
    
    public EmployeeRepository(EFContext context) : base(context)
    {
        _dbEmployee = context.Set<EmployeeEntity>();
        _context = context;
    }
}