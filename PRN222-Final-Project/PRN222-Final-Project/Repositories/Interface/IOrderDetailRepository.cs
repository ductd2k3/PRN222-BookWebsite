using PRN222_Final_Project.Models;

namespace PRN222_Final_Project.Repositories.Interface
{
    public interface IOrderDetailRepository : IGenericRepository<OrderDetail>
    {
        Task<IEnumerable<OrderDetail>> GetByOrderIdAsync(int orderId);
    }
}
