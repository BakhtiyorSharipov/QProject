using QDomain.Models;

namespace QApplication.Interfaces.Repository;

public interface IBlockedCustomerRepository
{
    IQueryable<BlockedCustomerEntity> GetAll(int pageList, int pageNumber);
    IQueryable<BlockedCustomerEntity> GetAllBlockedCustomersByCompany(int companyId);
    BlockedCustomerEntity FindById(int id);
    void Add(BlockedCustomerEntity entity);
    void Delete(BlockedCustomerEntity entity);
    int SaveChanges();
}