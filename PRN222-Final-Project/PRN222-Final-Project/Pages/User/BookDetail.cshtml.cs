using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_Final_Project.Models;
using PRN222_Final_Project.Services.Interface;

namespace PRN222_Final_Project.Pages.User
{
    public class BookDetailModel : PageModel
    {
        private readonly IGenericService<Product> _product;

        public BookDetailModel(IGenericService<Product> product)
        {
            _product = product;
        }

        [BindProperty]
        public Product Book { get; set; }
        public IEnumerable<Product> ProductRelated { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            Book = await _product.GetByIdAsync(id);
            if (Book == null)
            {
                return NotFound();
            }
            int? bookCatrgoryId = Book.CategoryId;
            ProductRelated = (await _product.GetAllAsync())
                    .Where(x => x.CategoryId == bookCatrgoryId)
                    .OrderByDescending(p => p.Price)
                    .Take(8)
                    .ToList();
            return Page();
        }
    }
}
