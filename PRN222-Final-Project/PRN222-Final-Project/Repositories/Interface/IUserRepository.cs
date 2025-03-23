using PRN222_Final_Project.Models;

namespace PRN222_Final_Project.Repositories.Interface
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> login(string email, string password);
        Task<(IEnumerable<User> Items, int TotalCount)> GetUserPagesAsync(
           string search = null,
           int pageNumber = 1,
           int pageSize = 10);
        Task<bool> IsDuplicateEmail(string email, string username);
    }
}
