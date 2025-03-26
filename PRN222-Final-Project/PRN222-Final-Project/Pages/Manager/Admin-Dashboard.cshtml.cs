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
        private readonly IGenericService<OrderDetail> _genericServiceOrderDetail;
        private readonly IUserService _userService;

        public Admin_DashboardModel(IBookService bookService
            , IGenericService<OrderDetail> genericServiceOrderDetail
            , IUserService userService)
        {
            _bookService = bookService;
            _genericServiceOrderDetail = genericServiceOrderDetail;
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

            var orderDetails = await _genericServiceOrderDetail.GetAllAsync();
            totalOrder = orderDetails?.Count() ?? 0;

            if (orderDetails != null)
            {
                foreach (var order in orderDetails)
                {
                    totalPrice += order.Quantity * order.UnitPrice;
                }
            }

            return Page();
        }
    }
}
