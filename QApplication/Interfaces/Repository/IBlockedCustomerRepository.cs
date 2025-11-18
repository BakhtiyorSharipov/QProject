using QDomain.Models;

namespace QApplication.Interfaces.Repository;

public interface IBlockedCustomerRepository
{
    IQueryable<BlockedCustomerEntity> GetAll(int pageList, int pageNumber);
    IQueryable<BlockedCustomerEntity> GetAllBlockedCustomersByCompany(int companyId);
    Task<BlockedCustomerEntity> FindByIdAsync(int id);
    Task AddAsync(BlockedCustomerEntity entity);
    void Delete(BlockedCustomerEntity entity);
    bool Exists(int customerId, int companyId);
    Task<int> SaveChangesAsync();
}