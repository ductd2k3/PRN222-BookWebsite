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
        private readonly IGenericService<Cart> _cart;

        public HomeModel(IGenericService<Category> categoryService, IGenericService<Product> product , IGenericService<Cart> cart)
        {
            _categoryService = categoryService;
            _product = product;
            _cart = cart;
        }

        public int? SelectedCategoryId { get; set; }
        public string SelectedPriceRange { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Product> Product { get; set; }
        public IEnumerable<Product> ProductSearch { get; set; }
        public Cart ProductCart { get; set; }

        public async Task OnGetDefault()
        {
            Categories = await _categoryService.GetAllAsync();
            Product = (await _product.GetAllAsync())
               .OrderByDescending(p => p.Price)
               .Take(4)
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

        public async Task<IActionResult> OnGetFilteredAsync(int? categoryId, string priceRange, int pageNumber = 1)
        {
            int pageSize = 6;
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

            int totalProducts = products.Count();
            ProductSearch = products.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            ViewData["PageNumber"] = pageNumber;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalProducts / pageSize);
            ViewData["SelectedCategoryId"] = categoryId;
            ViewData["SelectedPriceRange"] = priceRange;

            return Page();
        }


        // Add cart
        public async Task<IActionResult> OnGetAddToCartAsync(int? productID, int quantity)
        {
            string userIDStr = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userIDStr) || !int.TryParse(userIDStr, out int userID))
            {
                return RedirectToPage("/User/Login");
            }
            var cartItems = await _cart.GetAllAsync();
            var existingCart = cartItems.FirstOrDefault(x => x.ProductId == productID && x.UserId == userID);

            if (existingCart == null) 
            {
                Cart newCart = new Cart
                {
                    ProductId = productID,
                    UserId = userID,
                    Quantity = quantity,
                    AddedDate = DateTime.Now
                };

                await _cart.AddAsync(newCart);
            }
            else
            {
                existingCart.Quantity += quantity;
                await _cart.UpdateAsync(existingCart);
            }

            return RedirectToPage("/User/Cart");
        }



    }
}
