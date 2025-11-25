using QDomain.Models;

namespace QApplication.Interfaces.Repository;

public interface IUserRepository
{
    IQueryable<User> Queryable();
    User? FindById(int id);
    User? FindByEmail(string email);
    void Add(User entity);
    void Update(User entity);
    void Delete(User entity);
    int SaveChanges();
}