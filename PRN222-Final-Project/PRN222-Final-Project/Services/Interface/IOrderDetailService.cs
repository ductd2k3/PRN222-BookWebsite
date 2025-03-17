using PRN222_Final_Project.Models;

namespace PRN222_Final_Project.Services.Interface
{
    public interface IOrderDetailService : IGenericService<OrderDetail>
    {
        Task<IEnumerable<OrderDetail>> GetByOrderIdAsync(int orderId);
    }
}
