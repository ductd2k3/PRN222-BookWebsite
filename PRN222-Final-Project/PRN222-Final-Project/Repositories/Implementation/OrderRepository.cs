using Microsoft.EntityFrameworkCore;
using PRN222_Final_Project.Models;
using PRN222_Final_Project.Repositories.Interface;

namespace PRN222_Final_Project.Repositories.Implementation
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly BookStoreDbOptimizedContext _context;
        public OrderRepository(BookStoreDbOptimizedContext context) : base(context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Order> Orders, int totalItems)> GetOrderByPageAsync(int? orderId, int? statusId, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Orders.AsQueryable();

            if(orderId.HasValue)
            {
                query = query.Where(o => o.OrderId == orderId);
            }

            if(statusId.HasValue)
            {
                query = query.Where(o => o.StatusId == statusId);
            }

            int totalItems = await query.CountAsync();

            var items = await query
                .OrderByDescending(o => o.OrderId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalItems);
        }
    }
}
