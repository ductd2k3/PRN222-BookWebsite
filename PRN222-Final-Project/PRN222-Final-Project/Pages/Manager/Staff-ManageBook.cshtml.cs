using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using PRN222_Final_Project.Models;
using PRN222_Final_Project.Services.Interface;
using System.Net;

namespace PRN222_Final_Project.Pages.Manager
{
    public class Staff_ManageBookModel : PageModel
    {
        private readonly IBookService _bookService;
        private readonly IGenericService<Category> _categoryService;

        public Staff_ManageBookModel(IBookService bookService, IGenericService<Category> categoryService)
        {
            _bookService = bookService;
            _categoryService = categoryService;
        }

        public IEnumerable<Product> Books { get; private set; } = Enumerable.Empty<Product>();
        public IEnumerable<Category> Categories { get; private set; } = Enumerable.Empty<Category>();
        public int TotalCount { get; private set; }
        public int PageSize { get; } = 5;
        public int CurrentPage { get; private set; } = 1;

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }
        public string AlertMessage { get; set; }
        public async Task<IActionResult> OnGetAsync(int pageNumber = 1)
        {
            try
            {
                CurrentPage = Math.Max(1, pageNumber);
                var (books, totalCount) = await _bookService.GetBooksAsync(
                    SearchTerm, null, null, null, CurrentPage, PageSize);

                Books = books ?? Enumerable.Empty<Product>();
                TotalCount = totalCount;
                Categories = await _categoryService.GetAllAsync() ?? Enumerable.Empty<Category>();

                return Page();
            }
            catch (Exception ex)
            {
                // Log exception here in production
                return StatusCode(500);
            }
        }

        public async Task<IActionResult> OnPostAsync(IFormCollection form, IFormFile? imageFile)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await OnGetAsync();
                    AlertMessage = "Lỗi format vui lòng thử lại!";
                    return Page();
                }

                var book = await CreateBookFromFormAsync(form, imageFile);

                if (int.TryParse(form["bookId"], out int bookId) && bookId > 0)
                {
                    book.ProductId = bookId;
                    if(imageFile == null)
                    {
                        book.ImageUrl = form["currentImageUrl"];
                    }
                    await _bookService.UpdateAsync(book);
                    AlertMessage = "Chỉnh sửa thành công!";
                }
                else
                {
                    await _bookService.AddAsync(book);
                    AlertMessage = "Thêm thành công";
                }
                await OnGetAsync();
                
                return Page();
            }
            catch (Exception ex)
            {
                await OnGetAsync();
                return Page();
            }
        }

        private async Task<Product> CreateBookFromFormAsync(IFormCollection form, IFormFile? imageFile)
        {
            var book = new Product
            {
                Title = form["name"],
                Author = form["author"],
                Description = form["description"]
            };

            if (decimal.TryParse(form["price"], out decimal price))
                book.Price = price;

            if (int.TryParse(form["stock"], out int stock))
                book.Stock = stock;

            if (int.TryParse(form["categoryId"], out int categoryId))
                book.CategoryId = categoryId;

            if (imageFile?.Length > 0)
            {
                book.ImageUrl = await SaveImageAsync(imageFile);
            }

            return book;
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/books", fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(filePath)); // Ensure directory exists

            using var stream = new FileStream(filePath, FileMode.Create);
            await imageFile.CopyToAsync(stream);

            return $"/images/books/{fileName}";
        }
    }
}