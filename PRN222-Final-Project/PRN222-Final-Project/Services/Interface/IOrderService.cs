using PRN222_Final_Project.ModelDto;
using PRN222_Final_Project.Models;

namespace PRN222_Final_Project.Services.Interface
{
    public interface IOrderService : IGenericService<Order>
    {
        Task<(IEnumerable<Order> Orders, int totalItems)> GetOrderByPageAsync(
            int? orderId,
            int? statusId,
            int pageNumber = 1,
            int pageSize = 10
            );
        Task<OrderStatistic> GetOrderStatisticAsync();
    }
}
