﻿using Microsoft.EntityFrameworkCore;
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
            var user = await _context.Users
                .Include(u => u.Role) // Include Role để lấy RoleName
                .FirstOrDefaultAsync(u => u.Email == email || u.Username == email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return null;
            }

            return user;
        }

        public async Task<(IEnumerable<User> Items, int TotalCount)> GetUserPagesAsync(
           string search = null,
           int pageNumber = 1,
           int pageSize = 10)
        {
            var query = _context.Users.AsQueryable();

            if(search != null)
            {
                search = search.ToLower();
                query = query.Where(u => u.Username.ToLower().Contains(search) 
                                    || u.Email.ToLower().Contains(search));
            }

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.UserId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<bool> IsDuplicateEmail(string email, string Username)
        {
            return await _context.Users.AnyAsync(u => u.Email == email || u.Username == Username);
        }
    }
}
