using Microsoft.EntityFrameworkCore;
using PRN222_Final_Project.Models;
using PRN222_Final_Project.Repositories.Interface;

namespace PRN222_Final_Project.Repositories.Implementation
{
    public class OrderDetailRepository : GenericRepository<OrderDetail>, IOrderDetailRepository
    {
        private readonly BookStoreDbOptimizedContext _context;
        public OrderDetailRepository(BookStoreDbOptimizedContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderDetail>> GetByOrderIdAsync(int orderId)
        {
            return await _context.OrderDetails
                                 .Where(od => od.OrderId == orderId)
                                 .ToListAsync();
        }
    }
}
