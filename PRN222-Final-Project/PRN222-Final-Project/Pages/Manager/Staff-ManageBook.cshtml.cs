using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_Final_Project.Models;
using PRN222_Final_Project.Services.Interface;

namespace PRN222_Final_Project.Pages.Manager
{
    public class Staff_ManageBookModel : PageModel
    {
        private readonly IBookService _bookService;
        private readonly IGenericService<Category> _genericServiceCategory;

        public Staff_ManageBookModel(IBookService bookService, IGenericService<Category> genericServiceCategory)
        {
            _bookService = bookService;
            _genericServiceCategory = genericServiceCategory;
        }

        public IEnumerable<Product> Books { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; } = 5;
        public int CurrentPage { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        public async Task OnGetAsync(int pageNumber = 1)
        {
            CurrentPage = pageNumber;
            var (items, totalCount) = await _bookService.GetBooksAsync(
                SearchTerm, null, null, null, pageNumber, PageSize);
            Books = items;
            TotalCount = totalCount;
            Categories = await _genericServiceCategory.GetAllAsync();
        }

        public async Task<IActionResult> OnPostAsync(IFormCollection form, IFormFile imageFile)
        {
            // Tạo đối tượng Product từ dữ liệu form
            var book = new Product();

            // Lấy dữ liệu từ form
            book.Title = form["name"];
            book.Author = form["author"];
            if (decimal.TryParse(form["price"], out decimal price))
            {
                book.Price = price;
            }
            if (int.TryParse(form["stock"], out int stock))
            {
                book.Stock = stock;
            }
            if (int.TryParse(form["categoryId"], out int categoryId))
            {
                book.CategoryId = categoryId;
            }
            book.Description = form["description"];

            // Xử lý upload ảnh
            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/books", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                book.ImageUrl = "/images/books/" + fileName;
            }

            await _bookService.AddAsync(book);
            return RedirectToPage(); // Reload trang sau khi thêm thành công
        }

    }
}
