using PRN222_Final_Project.Models;

namespace PRN222_Final_Project.Services.Interface
{
    public interface IUserService : IGenericService<User>
    {
        Task<User> Login(string username, string password);
    }
}
