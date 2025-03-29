using Microsoft.EntityFrameworkCore;
using PRN222_Final_Project.ModelDto;
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

        public async Task<bool> IsDuplicateEmail(string email, string username)
        {
            return await _userRepository.IsDuplicateEmail(email, username);
        }
        public async Task<IEnumerable<TopCustomer>> GetTopUser(int top)
        {
            return await _userRepository.GetTopUser(top);
        }
        public async Task<int> GetTotalBuyer()
        {
            return await _userRepository.GetTotalBuyer();
        }

        public async Task<IEnumerable<StaffStatistic>> GetStaffStatistic(int top)
        {
            return await _userRepository.GetStaffStatistic(top);
        }
    }
}
