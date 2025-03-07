using PRN222_Final_Project.Models;

namespace PRN222_Final_Project.Services.Interface
{
    public interface IBookService : IGenericService<Product>
    {
        Task<(IEnumerable<Product> Items, int TotalCount)> GetBooksAsync(
        string searchTerm = null,
        int? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        int pageNumber = 1,
        int pageSize = 10);
    }
}
