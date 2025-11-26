using QDomain.Models;

namespace QApplication.Interfaces.Repository;

public interface IUserRepository
{
    Task<User?> FindByIdAsync(int id);
    Task<User?> FindByEmailAsync(string email);
    Task AddAsync(User entity);
    void Update(User entity);
    void Delete(User entity);
    Task<int> SaveChangesAsync();
}