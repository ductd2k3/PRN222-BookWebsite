using PRN222_Final_Project.Models;
using PRN222_Final_Project.Repositories.Interface;
using PRN222_Final_Project.Services.Interface;

namespace PRN222_Final_Project.Services.Implementation
{
    public class UserService : GenericService<User>, IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository) : base(userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> Login(string username, string password)
        {
            return await _userRepository.login(username, password);
        }
        public async Task<(IEnumerable<User> Items, int TotalCount)> GetUserPagesAsync(
           string search = null,
           int pageNumber = 1,
           int pageSize = 10)
        {
            return await _userRepository.GetUserPagesAsync(search, pageNumber, pageSize);
        }
    }
}
