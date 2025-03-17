using PRN222_Final_Project.Models;
using PRN222_Final_Project.Repositories.Interface;
using PRN222_Final_Project.Services.Interface;

namespace PRN222_Final_Project.Services.Implementation
{
    public class OrderDetailService : GenericService<OrderDetail>, IOrderDetailService
    {
        private readonly IOrderDetailRepository _orderDetailService;
        public OrderDetailService(IOrderDetailRepository repository) : base(repository)
        {
            _orderDetailService = repository;
        }

        public async Task<IEnumerable<OrderDetail>> GetByOrderIdAsync(int orderId)
        {
            return await _orderDetailService.GetByOrderIdAsync(orderId);
        }
    }
}
