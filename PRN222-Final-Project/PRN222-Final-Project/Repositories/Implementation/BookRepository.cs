using Microsoft.EntityFrameworkCore;
using PRN222_Final_Project.ModelDto;
using PRN222_Final_Project.Models;
using PRN222_Final_Project.Repositories.Interface;

namespace PRN222_Final_Project.Repositories.Implementation
{
    public class BookRepository : GenericRepository<Product>, IBookRepository
    {
        private readonly BookStoreDbOptimizedContext _context;
        public BookRepository(BookStoreDbOptimizedContext context) : base(context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Product> Items, int TotalCount)> GetBooksAsync(
        string searchTerm = null,
        int? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        int pageNumber = 1,
        int pageSize = 10)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            // Tìm kiếm
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(p => p.Title.ToLower().Contains(searchTerm) ||
                                        p.Author.ToLower().Contains(searchTerm));
            }

            // Lọc
            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            // Tổng số bản ghi
            int totalCount = await query.CountAsync();

            // Phân trang
            var items = await query
                .OrderByDescending(p => p.ProductId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<ProductStatistic> GetBookStatisticAsync(int top)
        {
            var TotalBook = await _context.OrderDetails
                                    .Where(od => od.Order.StatusId == 3)
                                    .SumAsync(od => od.Quantity);

            var TopProductsSeller = await _context.OrderDetails
                                                .Where(od => od.Order.StatusId == 3)
                                                .GroupBy(od => od.Product)
                                                .Select(g => new ProductStatisticDTO
                                                {
                                                    ProductId = g.Key.ProductId,
                                                    Title = g.Key.Title,
                                                    TotalSold = g.Sum(od => od.Quantity),
                                                })
                                                .OrderByDescending(x => x.TotalSold)
                                                .Take(top)
                                                .ToListAsync();

            var bestCategory = await _context.OrderDetails
                .Where(od => od.Order.StatusId == 3)
                .GroupBy(od => od.Product.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    TotalSold = g.Sum(od => od.Quantity)
                })
                .OrderByDescending(x => x.TotalSold)
                .Select(x => x.Category)
                .FirstOrDefaultAsync();

            return new ProductStatistic
            {
                TotalBook = TotalBook,
                TopProductsSeller = TopProductsSeller,
                BestCategory = bestCategory
            };
        }
    }
}
