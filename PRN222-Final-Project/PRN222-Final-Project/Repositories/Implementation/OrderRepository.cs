using Microsoft.EntityFrameworkCore;
using PRN222_Final_Project.ModelDto;
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

        public async Task<OrderStatistic> GetOrderStatisticAsync()
        {
            var TotalAmount = await _context.Orders
                                        .Where(o => o.StatusId == 3)
                                        .SumAsync(o => o.TotalAmount);
            var TotalOrder = await _context.Orders.CountAsync();
            var CompletedOrder = await _context.Orders.CountAsync(o => o.StatusId == 3);
            var ProcessedOrder = await _context.Orders.CountAsync(o => o.StatusId == 1);
            var AverageOrderValue = await _context.Orders.Where(o => o.StatusId == 3).AverageAsync(o => o.TotalAmount);
            return new OrderStatistic { 
                TotalOrder = TotalOrder,
                CompletedOrder = CompletedOrder,
                TotalAmount = TotalAmount,
                AverageOrderValue = AverageOrderValue
            };
        }
    }
}
