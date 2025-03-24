using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using PRN222_Final_Project.Hubs;
using PRN222_Final_Project.ModelDto;
using PRN222_Final_Project.Models;
using PRN222_Final_Project.Services.Interface;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PRN222_Final_Project.Pages.User
{
    public class CartModel : PageModel
    {
        private readonly IGenericService<Cart> _cart;
        private readonly IGenericService<Product> _product;
        private readonly IGenericService<Order> _order;
        private readonly IGenericService<OrderDetail> _orderDetail;
        private readonly IHubContext<OrderHub> _hubContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public CartModel(IGenericService<Cart> cart,
                         IGenericService<Product> product,
                         IGenericService<Order> order,
                         IGenericService<OrderDetail> orderDetail,
                         IHubContext<OrderHub> hubContext,
                         IHttpClientFactory httpClientFactory,
                         IConfiguration configuration,
                         IEmailService emailService)
        {
            _cart = cart;
            _product = product;
            _order = order;
            _orderDetail = orderDetail;
            _hubContext = hubContext;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _emailService = emailService;
        }

        public List<CartViewModel> ListCart { get; set; }
        public int PageSize { get; set; } = 3;
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

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
            var filteredCart = (from cart in cartItems
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
            TotalPages = (int)Math.Ceiling((double)filteredCart.Count / PageSize);
            ListCart = filteredCart.Skip((pageNumber - 1) * PageSize).Take(PageSize).ToList();

            CurrentPage = pageNumber;

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
            var product = await _product.GetByIdAsync(productId);

            if (product == null)
            {
                TempData["ErrorMessage"] = "Sản phẩm không tồn tại!";
                return RedirectToPage();
            }

            if (cartItem != null)
            {
                int newQuantity = cartItem.Quantity + change;

                if (newQuantity > product.Stock)
                {
                    TempData["ErrorMessage"] = "Số lượng sản phẩm trong giỏ vượt quá số lượng còn lại!";
                    return RedirectToPage();
                }

                if (newQuantity <= 0)
                {
                    await _cart.DeleteAsync(cartItem.CartId);
                }
                else
                {
                    cartItem.Quantity = newQuantity;
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

        public async Task<IActionResult> OnPostPayment(string address)
        {
            string userIdString = HttpContext.Session.GetString("UserID");
            string userName = HttpContext.Session.GetString("UserName") ?? "Khách hàng";
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToPage("/User/Login");
            }

            int userID = int.Parse(userIdString);
            var cartItems = await _cart.GetAllAsync();
            var products = await _product.GetAllAsync();

            var listCart = (from cart in cartItems
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

            if (!listCart.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống!";
                return RedirectToPage();
            }

            decimal totalAmount = listCart.Sum(x => x.Quantity * x.Price) ?? 0;
            var newOrder = new Order
            {
                UserId = userID,
                OrderDate = DateTime.Now,
                TotalAmount = totalAmount,
                StatusId = 1, // Chờ xác nhận
                ShippingAddress = address,
                PaymentMethod = "Bank Transfer" // Chuyển khoản ngân hàng
            };

            // Thêm đơn hàng
            await _order.AddAsync(newOrder);
            int orderId = newOrder.OrderId;

            // Thêm chi tiết đơn hàng và cập nhật kho
            foreach (var item in listCart)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = orderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity ?? 0,
                    UnitPrice = item.Price
                };
                await _orderDetail.AddAsync(orderDetail);

                var product = products.FirstOrDefault(p => p.ProductId == item.ProductId);
                if (product != null)
                {
                    product.Stock -= item.Quantity ?? 0;
                    await _product.UpdateAsync(product);
                }
            }

            // Thông báo qua SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveOrderNotification", orderId, userName, totalAmount);

            return Page();
        }

        public async Task<IActionResult> OnGetCheckPayment(string paymentCode, string address)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var apiKey = _configuration["Sepay:ApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                {
                    return new JsonResult(new { success = false, message = "API key không được cấu hình" });
                }

                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                var url = "https://my.sepay.vn/userapi/transactions/list?account_number=00003480126&limit=20";
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new JsonResult(new { success = false, message = $"Không thể kết nối tới Sepay API: {response.StatusCode} - {errorContent}" });
                }

                var json = await response.Content.ReadAsStringAsync();
                var sepayResponse = JsonSerializer.Deserialize<SepayTransactionResponse>(json);

                if (sepayResponse?.Status != 200 || sepayResponse?.Messages?.Success != true)
                {
                    return new JsonResult(new { success = false, message = "Phản hồi từ Sepay không thành công" });
                }

                var matchedTransaction = sepayResponse.Transactions?.FirstOrDefault(tx =>
                    tx.TransactionContent?.Trim().ToLower() == paymentCode?.Trim().ToLower());

                if (matchedTransaction != null)
                {
                    await OnPostPayment(address); // Xử lý đơn hàng khi thanh toán khớp
                    return new JsonResult(new { success = true, message = "Thanh toán thành công!" });
                }

                return new JsonResult(new { success = false, message = "Chưa tìm thấy giao dịch khớp" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Lỗi hệ thống: {ex.Message}" });
            }
        }

        public async Task<IActionResult> OnGetSendConfirmationEmail(string address)
        {
            try
            {
                string userIdString = HttpContext.Session.GetString("UserID");
                if (string.IsNullOrEmpty(userIdString))
                {
                    return new JsonResult(new { success = false, message = "Người dùng chưa đăng nhập" });
                }

                int userId = int.Parse(userIdString);
                var userEmail = HttpContext.Session.GetString("UserEmail");
                var userName = HttpContext.Session.GetString("UserName") ?? "Khách hàng";

                var cartItems = await _cart.GetAllAsync();
                var products = await _product.GetAllAsync();
                var orders = await _order.GetAllAsync();
                var order = orders.Where(o => o.UserId == userId && o.ShippingAddress == address)
                                .OrderByDescending(o => o.OrderDate)
                                .FirstOrDefault();

                if (order == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy đơn hàng" });
                }

                var cartDetails = (from cart in cartItems
                                   join product in products on cart.ProductId equals product.ProductId
                                   where cart.UserId == userId
                                   select new CartViewModel
                                   {
                                       ProductName = product.Title,
                                       Quantity = cart.Quantity,
                                       Price = product.Price
                                   }).ToList();

                // Tạo nội dung email chuyên nghiệp
                string orderDetailsHtml = string.Join("", cartDetails.Select(item =>
                    $"<tr><td>{item.ProductName}</td><td>{item.Quantity}</td><td>{item.Price:N0} đ</td><td>{(item.Quantity * item.Price):N0} đ</td></tr>"));
                decimal totalAmount = cartDetails.Sum(x => x.Quantity * x.Price) ?? 0;

                var emailMessage = new EmailMessage
                {
                    To = userEmail,
                    Subject = $"Xác Nhận Đơn Hàng #{order.OrderId} Thành Công",
                    Body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; color: #333; line-height: 1.6; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: #4CAF50; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background: #ffffff; padding: 20px; border: 1px solid #eee; border-radius: 0 0 5px 5px; }}
        .footer {{ text-align: center; font-size: 12px; color: #777; margin-top: 20px; }}
        table {{ width: 100%; border-collapse: collapse; margin: 20px 0; }}
        th, td {{ padding: 10px; border: 1px solid #ddd; text-align: left; }}
        th {{ background: #f5f5f5; }}
        .total {{ font-weight: bold; font-size: 1.1em; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>Xin chào {userName},</h2>
            <p>Đơn hàng của bạn đã được xác nhận thành công!</p>
        </div>
        <div class='content'>
            <h3>Thông tin đơn hàng #{order.OrderId}</h3>
            <p><strong>Ngày đặt hàng:</strong> {order.OrderDate:dd/MM/yyyy HH:mm}</p>
            <p><strong>Địa chỉ giao hàng:</strong> {address}</p>
            <p><strong>Phương thức thanh toán:</strong> Chuyển khoản ngân hàng</p>
            
            <table>
                <thead>
                    <tr>
                        <th>Sản phẩm</th>
                        <th>Số lượng</th>
                        <th>Đơn giá</th>
                        <th>Thành tiền</th>
                    </tr>
                </thead>
                <tbody>
                    {orderDetailsHtml}
                </tbody>
            </table>
            
            <p class='total'>Tổng cộng: {totalAmount:N0} đ</p>
            
            <p>Cảm ơn bạn đã mua sắm tại cửa hàng của chúng tôi. Đơn hàng sẽ được giao đến bạn trong thời gian sớm nhất.</p>
            <p>Nếu có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi qua email: support@shop.com hoặc số điện thoại: 0123-456-789.</p>
        </div>
        <div class='footer'>
            <p>Trân trọng,<br/>Đội ngũ Shop<br/>© 2025 Shop. All rights reserved.</p>
        </div>
    </div>
</body>
</html>",
                    IsHtml = true
                };
                // Xóa giỏ hàng
                foreach (var item in cartItems.Where(c => c.UserId == userId))
                {
                    await _cart.DeleteAsync(item.CartId);
                }
                await _emailService.SendEmailAsync(emailMessage);
                return new JsonResult(new { success = true, message = "Email xác nhận đã được gửi" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Lỗi khi gửi email: {ex.Message}" });
            }
        }

        public class SepayTransactionResponse
        {
            [JsonPropertyName("status")]
            public int Status { get; set; }

            [JsonPropertyName("error")]
            public object Error { get; set; }

            [JsonPropertyName("messages")]
            public Messages Messages { get; set; }

            [JsonPropertyName("transactions")]
            public List<Transaction> Transactions { get; set; }
        }

        public class Messages
        {
            [JsonPropertyName("success")]
            public bool Success { get; set; }
        }

        public class Transaction
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("transaction_content")]
            public string TransactionContent { get; set; }
        }
    }
}