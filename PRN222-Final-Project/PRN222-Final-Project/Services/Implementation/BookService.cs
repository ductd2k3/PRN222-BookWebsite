using PRN222_Final_Project.Models;
using PRN222_Final_Project.Repositories.Interface;
using PRN222_Final_Project.Services.Interface;

namespace PRN222_Final_Project.Services.Implementation
{
    public class BookService : GenericService<Product>, IBookService
    {
        private readonly IBookRepository _bookRepository;
        public BookService(IBookRepository bookRepository) : base(bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public Task<(IEnumerable<Product> Items, int TotalCount)> GetBooksAsync(
        string searchTerm = null,
        int? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        int pageNumber = 1,
        int pageSize = 10)
        {
            return _bookRepository.GetBooksAsync(searchTerm, categoryId, minPrice, maxPrice, pageNumber, pageSize);
        }
    }
}
