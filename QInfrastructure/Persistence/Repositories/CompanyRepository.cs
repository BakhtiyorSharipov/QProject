using Microsoft.EntityFrameworkCore;
using QApplication.Interfaces.Repository;
using QDomain.Models;
using QInfrastructure.Persistence.DataBase;

namespace QInfrastructure.Persistence.Repositories;

public class CompanyRepository: Repository<CompanyEntity>, ICompanyRepository
{
    private readonly DbSet<CompanyEntity> _dbCompany;
    private readonly EFContext _context;
    
    public CompanyRepository(EFContext context) : base(context)
    {
        _dbCompany = context.Set<CompanyEntity>();
        _context = context;
    }
}