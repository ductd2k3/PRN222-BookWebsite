using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_Final_Project.Models;
using PRN222_Final_Project.Services.Interface;

namespace PRN222_Final_Project.Pages.Manager
{
    public class Staff_ManageOrderDetailModel : PageModel
    {
        private readonly IOrderDetailService _orderDetailService;
        private readonly IOrderService _orderService;
        private readonly IGenericService<OrderStatus> _orderStatusService;

        public Staff_ManageOrderDetailModel(IOrderDetailService orderDetailService, IGenericService<OrderStatus> orderStatusService, IOrderService orderService)
        {
            _orderDetailService = orderDetailService;
            _orderStatusService = orderStatusService;
            _orderService = orderService;
        }
        public IEnumerable<OrderDetail> OrderDetails { get; set; } = Enumerable.Empty<OrderDetail>();
        public Order Order { get; set; }
        public async Task<IActionResult> OnGetAsync(int? orderId)
        {
            int id = orderId.HasValue ? orderId.Value : 1;
            OrderDetails = await _orderDetailService.GetByOrderIdAsync(id);
            Order = await _orderService.GetByIdAsync(id);
            if (OrderDetails == null)
            {
                return RedirectToAction("/Manager/Staff-ManageOrder");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(IFormCollection form)
        {
            var status = form["status"];
            int.TryParse(form["orderId"], out int id);
            var order = await _orderService.GetByIdAsync(id);
            if (status.Equals("confirm"))
            {
                order.StatusId = 3;
            }
            else
            {
                order.StatusId = 4;
            }
            await _orderService.UpdateAsync(order);
            await OnGetAsync(id);
            return Page();
        }
    }
}
