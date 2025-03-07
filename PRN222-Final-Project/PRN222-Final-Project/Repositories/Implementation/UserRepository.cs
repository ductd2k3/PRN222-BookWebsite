using Microsoft.EntityFrameworkCore;
using PRN222_Final_Project.Models;
using PRN222_Final_Project.Repositories.Interface;

namespace PRN222_Final_Project.Repositories.Implementation
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly BookStoreDbOptimizedContext _context;
        public UserRepository(BookStoreDbOptimizedContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User> login(string email, string password)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
        }
    }
}
