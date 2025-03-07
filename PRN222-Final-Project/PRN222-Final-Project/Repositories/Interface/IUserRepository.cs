using PRN222_Final_Project.Models;

namespace PRN222_Final_Project.Repositories.Interface
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> login(string email, string password);
    }
}
