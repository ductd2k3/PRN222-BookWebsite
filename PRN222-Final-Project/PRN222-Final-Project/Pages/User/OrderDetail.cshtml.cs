using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_Final_Project.ModelDto;
using PRN222_Final_Project.Models;
using PRN222_Final_Project.Services.Interface;

namespace PRN222_Final_Project.Pages.User
{
    public class OrderDetailModel : PageModel
    {
        private readonly IGenericService<Order> _order;
        private readonly IGenericService<OrderStatus> _orderStatus;
        private readonly IGenericService<OrderDetail> _orderDetail;
        private readonly IGenericService<Product> _product;

        public OrderDetailModel(IGenericService<Order> order, IGenericService<OrderStatus> orderStatus, IGenericService<OrderDetail> orderDetail, IGenericService<Product> product)
        {
            _order = order;
            _orderStatus = orderStatus;
            _orderDetail = orderDetail;
            _product = product;
        }

        public OrderViewModel inforOrder { get; set; }
        public List<OrderDetailViewModel> listOrderDetail { get; set; }

        public async Task<IActionResult> OnGet(int orderID)
        {
            string userIdString = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToPage("/User/Login");
            }
            int userID = int.Parse(userIdString);

            // Order
            var listOrder = await _order.GetAllAsync();
            var listOrderStatus = await _orderStatus.GetAllAsync();
            inforOrder = (from order in listOrder
                          join orderStatus in listOrderStatus
                          on order.StatusId equals orderStatus.StatusId
                          where order.OrderId == orderID && order.UserId == userID
                          select new OrderViewModel
                          {
                              OrderId = order.OrderId,
                              OrderDate = order.OrderDate,
                              OrderStatusID = order.StatusId,
                              Amount = order.TotalAmount,
                              OrderStatusName = orderStatus.StatusName,
                              Address = order.ShippingAddress
                          }).FirstOrDefault();
            // Order detail
            var listProduct = await _product.GetAllAsync();
            var listOrderDetails = await _orderDetail.GetAllAsync();

            listOrderDetail = (from orderDetail in listOrderDetails
                               join product in listProduct
                               on orderDetail.ProductId equals product.ProductId
                               where orderDetail.OrderId == orderID
                               select new OrderDetailViewModel
                               {
                                   PrudctName = product.Title,
                                   Quantity = orderDetail.Quantity,
                                   Price = orderDetail.UnitPrice,
                                   Image = product.ImageUrl
                               }).ToList();

            return Page();
        }

    }
}
