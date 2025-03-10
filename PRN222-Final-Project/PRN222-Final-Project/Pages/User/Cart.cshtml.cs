using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_Final_Project.ModelDto;
using PRN222_Final_Project.Models;
using PRN222_Final_Project.Services.Interface;
using System.Linq;
using System.Threading.Tasks;

namespace PRN222_Final_Project.Pages.User
{
    public class CartModel : PageModel
    {
        private readonly IGenericService<Cart> _cart;
        private readonly IGenericService<Product> _product;

        public CartModel(IGenericService<Cart> cart, IGenericService<Product> product)
        {
            _cart = cart;
            _product = product;
        }

        public List<CartViewModel> ListCart { get; set; }
        public int PageSize { get; set; } = 5; // Số sản phẩm trên mỗi trang
        public int CurrentPage { get; set; }

        public async Task<IActionResult> OnGet(int pageNumber = 1)
        {
            string userIdString = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToPage("/User/Login");
            }

            int userID = int.Parse(userIdString);
            var cartItems = await _cart.GetAllAsync();
            var products = await _product.GetAllAsync();

            ListCart = (from cart in cartItems
                        join product in products on cart.ProductId equals product.ProductId
                        where cart.UserId == userID
                        select new CartViewModel
                        {
                            ProductId = cart.ProductId,
                            UserId = cart.UserId,
                            Quantity = cart.Quantity,
                            AddedDate = cart.AddedDate,
                            ProductName = product.Title,
                            Price = product.Price
                        }).ToList();
            CurrentPage = pageNumber;
            ListCart = ListCart.Skip((pageNumber - 1) * PageSize).Take(PageSize).ToList();

            return Page();
        }
        public async Task<IActionResult> OnPostUpdateQuantity(int productId, int change)
        {
            string userIdString = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToPage("/User/Login");
            }

            int userID = int.Parse(userIdString);
            var cartItem = (await _cart.GetAllAsync()).FirstOrDefault(c => c.UserId == userID && c.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity += change;
                if (cartItem.Quantity <= 0)
                {
                    await _cart.DeleteAsync(cartItem.CartId);
                }
                else
                {
                    await _cart.UpdateAsync(cartItem);
                }
            }

            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostRemoveItem(int productId)
        {
            string userIdString = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToPage("/User/Login");
            }

            int userID = int.Parse(userIdString);
            var cartItem = (await _cart.GetAllAsync()).FirstOrDefault(c => c.UserId == userID && c.ProductId == productId);

            if (cartItem != null)
            {
                await _cart.DeleteAsync(cartItem.CartId);
            }

            return RedirectToPage();
        }
    }
}
