using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_Final_Project.Models;
using PRN222_Final_Project.Services.Interface;

namespace PRN222_Final_Project.Pages.User
{
    public class HomeModel : PageModel
    {
        private readonly IGenericService<Category> _categoryService;
        private readonly IGenericService<Product> _product;

        public HomeModel(IGenericService<Category> categoryService, IGenericService<Product> product)
        {
            _categoryService = categoryService;
            _product = product;
        }

        public int? SelectedCategoryId { get; set; }
        public string SelectedPriceRange { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Product> Product { get; set; }
        public IEnumerable<Product> ProductSearch { get; set; }

        public async Task OnGetDefault()
        {
            Categories = await _categoryService.GetAllAsync();
            Product = (await _product.GetAllAsync())
               .OrderByDescending(p => p.Price)
               .Take(8)
               .ToList();
        }

        public async Task OnGetAsync()
        {
            await OnGetDefault();
            ProductSearch = (await _product.GetAllAsync())
                   .OrderByDescending(p => p.Price)
                   .Take(3)
                   .ToList();
        }

        public async Task<IActionResult> OnGetFilteredAsync(int? categoryId, string priceRange)
        {
            var products = await _product.GetAllAsync();
            await OnGetDefault();
            SelectedCategoryId = categoryId;
            SelectedPriceRange = priceRange;

            if (categoryId.HasValue && categoryId > 0)
            {
                products = products.Where(p => p.CategoryId == categoryId);
            }

            if (!string.IsNullOrEmpty(priceRange))
            {
                switch (priceRange)
                {
                    case "under_100":
                        products = products.Where(p => p.Price < 100000);
                        break;
                    case "100_200":
                        products = products.Where(p => p.Price >= 100000 && p.Price <= 200000);
                        break;
                    case "200_500":
                        products = products.Where(p => p.Price > 200000 && p.Price <= 500000);
                        break;
                    case "over_500":
                        products = products.Where(p => p.Price > 500000);
                        break;
                }
            }

            ProductSearch = products.ToList();
            return Page();
        }

    }
}
