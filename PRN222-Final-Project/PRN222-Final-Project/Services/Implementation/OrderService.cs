using PRN222_Final_Project.ModelDto;
using PRN222_Final_Project.Models;
using PRN222_Final_Project.Repositories.Interface;
using PRN222_Final_Project.Services.Interface;

namespace PRN222_Final_Project.Services.Implementation
{
    public class OrderService : GenericService<Order>, IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        public OrderService(IOrderRepository repository) : base(repository)
        {
            _orderRepository = repository;
        }

        public async Task<(IEnumerable<Order> Orders, int totalItems)> GetOrderByPageAsync(int? orderId, int? statusId, int pageNumber = 1, int pageSize = 10)
        {
            return await _orderRepository.GetOrderByPageAsync(orderId, statusId, pageNumber, pageSize);
        }
        public async Task<OrderStatistic> GetOrderStatisticAsync()
        {
            return await _orderRepository.GetOrderStatisticAsync();
        }
    }
}
