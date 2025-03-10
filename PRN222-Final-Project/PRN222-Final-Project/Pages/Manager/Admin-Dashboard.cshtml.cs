using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_Final_Project.Models;
using PRN222_Final_Project.Services.Interface;
using System.Reflection.Metadata.Ecma335;

namespace PRN222_Final_Project.Pages.Manager
{
    public class Admin_DashboardModel : PageModel
    {
        private readonly IBookService _bookService;
        private readonly IGenericService<Cart> genericServiceCart;
        private readonly IUserService _userService;

        public Admin_DashboardModel(IBookService bookService
            , IGenericService<Cart> genericServiceCart
            , IUserService userService)
        {
            _bookService = bookService;
            this.genericServiceCart = genericServiceCart;
            _userService = userService;
        }

        public int totalBooks { get; set; } = 0;
        public int totalUsers { get; set; } = 0;
        public int totalOrder { get; set; } = 0;
        public decimal totalPrice { get; set; } = 0;
        public async Task<IActionResult> OnGetAsync()
        {
            var books = await _bookService.GetAllAsync();
            totalBooks = books?.Count() ?? 0;

            var users = await _userService.GetAllAsync();
            totalUsers = users?.Count() ?? 0;

            var carts = await genericServiceCart.GetAllAsync();
            totalOrder = carts?.Count() ?? 0;

            if (carts != null)
            {
                foreach (var cart in carts)
                {
                    totalPrice += cart.Quantity * cart.Product.Price;
                }
            }

            return Page();
        }
    }
}
