using Microsoft.EntityFrameworkCore;
using QApplication.Interfaces.Repository;
using QDomain.Models;
using QInfrastructure.Persistence.DataBase;

namespace QInfrastructure.Persistence.Repositories;

public class CustomerRepository: Repository<CustomerEntity>, ICustomerRepository
{
    private readonly DbSet<CustomerEntity> _dbCustomer;
    private readonly EFContext _context;

    public CustomerRepository(EFContext context): base(context)
    {
        _dbCustomer = context.Set<CustomerEntity>();
        _context = context;
    }

}