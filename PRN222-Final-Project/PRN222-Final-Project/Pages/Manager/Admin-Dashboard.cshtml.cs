using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_Final_Project.ModelDto;
using PRN222_Final_Project.Models;
using PRN222_Final_Project.Services.Interface;
using System.Reflection.Metadata.Ecma335;

namespace PRN222_Final_Project.Pages.Manager
{
    public class Admin_DashboardModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        private readonly IBookService _bookService;
        public Admin_DashboardModel(IUserService userService
            ,IOrderService orderService
            ,IBookService bookService)
        {
            _userService = userService;
            _orderService = orderService;
            _bookService = bookService;
        }
        public IEnumerable<TopCustomer> TopUsers {  get; set; }
        public int TotalBuyer {  get; set; }
        public OrderStatistic OrderStatistic { get; set; }
        public ProductStatistic ProductStatistic { get; set; }
        public IEnumerable<StaffStatistic> StaffStatistics { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            TotalBuyer = await _userService.GetTotalBuyer();
            TopUsers = await _userService.GetTopUser(2);
            OrderStatistic = await _orderService.GetOrderStatisticAsync();
            ProductStatistic = await _bookService.GetBookStatisticAsync(3);
            StaffStatistics = await _userService.GetStaffStatistic(4);
            return Page();
        }
    }
}
