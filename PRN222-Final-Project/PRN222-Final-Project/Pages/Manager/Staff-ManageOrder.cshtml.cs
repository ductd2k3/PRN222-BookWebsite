using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_Final_Project.Models;
using PRN222_Final_Project.Services.Implementation;
using PRN222_Final_Project.Services.Interface;
using static System.Reflection.Metadata.BlobBuilder;

namespace PRN222_Final_Project.Pages.Manager
{
    public class Staff_ManageOrderModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly IGenericService<OrderStatus> _genericService;
        public Staff_ManageOrderModel(IOrderService orderService, IGenericService<OrderStatus> genericService)
        {
            _orderService = orderService;
            _genericService = genericService;
        }
        public IEnumerable<Order> Orders { get; set; } = Enumerable.Empty<Order>();
        public IEnumerable<OrderStatus> Statuss { get; set; } = Enumerable.Empty<OrderStatus>();
        public int TotalCount { get; private set; }
        public int PageSize { get; } = 5;
        public int CurrentPage { get; private set; } = 1;
        [BindProperty(SupportsGet = true)]
        public int? OrderId { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? StatusId { get; set; }
        public async Task<IActionResult> OnGetAsync(int pageNumber = 1)
        {
            try
            {
                CurrentPage = Math.Max(1, pageNumber);
                var (orders, totalItems) = await _orderService.GetOrderByPageAsync(OrderId, StatusId, CurrentPage, PageSize);

                Orders = orders ?? Enumerable.Empty<Order>();
                TotalCount = totalItems;
                Statuss = await _genericService.GetAllAsync() ?? Enumerable.Empty<OrderStatus>();

                return Page();
            }
            catch (Exception ex)
            {
                // Log exception here in production
                return StatusCode(500);
            }
        }
    }
}
