using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_Final_Project.ModelDto;
using PRN222_Final_Project.Models;
using PRN222_Final_Project.Services.Interface;

namespace PRN222_Final_Project.Pages.User
{
    public class OrderHistoryModel : PageModel
    {
        private readonly IGenericService<Order> _order;
        private readonly IGenericService<OrderStatus> _orderStatus;

        public OrderHistoryModel(IGenericService<Order> order, IGenericService<OrderStatus> orderStatus)
        {
            _order = order;
            _orderStatus = orderStatus;
        }

        public List<OrderHistoryViewModel> ListOrderHistory { get; set; } = new List<OrderHistoryViewModel>();
        public List<OrderStatus> ListOrderStatus { get; set; } = new List<OrderStatus>();
        public int? StatusFilter { get; set; }

        public int PageSize { get; } = 5;
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public async Task<IActionResult> OnGet(int? statusFilter, int pageNumber = 1)
        {
            string userIdString = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToPage("/User/Login");
            }

            StatusFilter = statusFilter;
            int userID = int.Parse(userIdString);
            var listOrder = await _order.GetAllAsync() ?? new List<Order>();
            var listOrderStatus = (await _orderStatus.GetAllAsync())?.ToList() ?? new List<OrderStatus>();

            ListOrderStatus = listOrderStatus;
            var filteredOrders = listOrder.Where(o => o.UserId == userID);

            if (statusFilter.HasValue && statusFilter.Value != 0)
            {
                filteredOrders = filteredOrders.Where(o => o.StatusId == statusFilter.Value);
            }

            var orderList = (from order in filteredOrders
                             join orderStatus in listOrderStatus on order.StatusId equals orderStatus.StatusId
                             select new OrderHistoryViewModel
                             {
                                 OrderId = order.OrderId,
                                 OrderDate = order.OrderDate,
                                 OrderStatusID = order.StatusId,
                                 Amount = order.TotalAmount,
                                 OrderStatusName = orderStatus.StatusName
                             }).ToList();

            TotalPages = (int)Math.Ceiling(orderList.Count / (double)PageSize);
            CurrentPage = pageNumber;
            ListOrderHistory = orderList.Skip((pageNumber - 1) * PageSize).Take(PageSize).ToList();

            return Page();
        }
    }

}
