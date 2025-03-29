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
using static System.Net.Mime.MediaTypeNames;

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
        private readonly IGenericService<Models.User> _user;

        public CartModel(IGenericService<Cart> cart,
                         IGenericService<Product> product,
                         IGenericService<Order> order,
                         IGenericService<OrderDetail> orderDetail,
                         IHubContext<OrderHub> hubContext,
                         IHttpClientFactory httpClientFactory,
                         IConfiguration configuration,
                         IEmailService emailService,
                         IGenericService<Models.User> user)
        {
            _cart = cart;
            _product = product;
            _order = order;
            _orderDetail = orderDetail;
            _hubContext = hubContext;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _emailService = emailService;
            _user = user;
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
                                    ProductId = cart.ProductId ?? 0, // Xử lý nullable
                                    UserId = cart.UserId,
                                    Quantity = cart.Quantity,
                                    AddedDate = cart.AddedDate,
                                    ProductName = product.Title,
                                    Price = product.Price,
                                    Image = product.ImageUrl
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
        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        public async Task<IActionResult> OnPostPayment(string address, int[] selectedItems)
        {
            string userIdString = HttpContext.Session.GetString("UserID");
            string userName = HttpContext.Session.GetString("UserName") ?? "Khách hàng";
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToPage("/User/Login");
            }

            if (selectedItems == null || !selectedItems.Any())
            {
                TempData["ErrorMessage"] = "Vui lòng chọn ít nhất một sản phẩm để thanh toán!";
                return RedirectToPage();
            }

            int userID = int.Parse(userIdString);
            var cartItems = await _cart.GetAllAsync();
            var products = await _product.GetAllAsync();
            var users = await _user.GetAllAsync();
            var currentUser = users.FirstOrDefault(u => u.UserId == userID);
            if (currentUser == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin người dùng!";
                return RedirectToPage();
            }
            var selectedItemsList = selectedItems.ToList();
            var listCart = (from cart in cartItems
                            join product in products on cart.ProductId equals product.ProductId
                            where cart.UserId == userID && cart.ProductId.HasValue && selectedItemsList.Contains(cart.ProductId.Value)
                            select new CartViewModel
                            {
                                ProductId = cart.ProductId ?? 0,
                                UserId = cart.UserId,
                                Quantity = cart.Quantity,
                                AddedDate = cart.AddedDate,
                                ProductName = product.Title,
                                Price = product.Price
                            }).ToList();

            if (!listCart.Any())
            {
                TempData["ErrorMessage"] = "Vui lòng chọn ít nhất một sản phẩm để thanh toán!";
                return RedirectToPage();
            }
            decimal totalAmount = listCart.Sum(x => x.Quantity * x.Price) ?? 0;
            bool canProceedWithOrder = true;
            string errorMessage = string.Empty;
            await semaphore.WaitAsync();
            try
            {
                foreach (var item in listCart)
                {
                    var product = products.FirstOrDefault(p => p.ProductId == item.ProductId);
                    if (product == null)
                    {
                        canProceedWithOrder = false;
                        errorMessage = $"Sản phẩm {item.ProductName} không tồn tại!";
                        break;
                    }
                    if (product.Stock < item.Quantity)
                    {
                        var otherCartItems = cartItems.Where(c => c.ProductId == item.ProductId && c.UserId != userID).ToList();
                        if (otherCartItems.Any())
                        {
                            var otherUserIds = otherCartItems.Select(c => c.UserId).Distinct().ToList();
                            var otherUsers = users.Where(u => otherUserIds.Contains(u.UserId)).ToList();
                            var higherPriorityUser = otherUsers.FirstOrDefault(u => u.TotalAmount > (currentUser.TotalAmount ?? 0));
                            if (higherPriorityUser != null)
                            {
                                canProceedWithOrder = false;
                                errorMessage = $"Sản phẩm {item.ProductName} đã hết hàng! Ưu tiên cho người dùng có điểm mua hàng cao hơn.";
                                break;
                            }
                        }
                        if (product.Stock < item.Quantity)
                        {
                            canProceedWithOrder = false;
                            errorMessage = $"Sản phẩm {item.ProductName} đã hết hàng!";
                            break;
                        }
                    }
                }
                if (canProceedWithOrder)
                {
                    var newOrder = new Order
                    {
                        UserId = userID,
                        OrderDate = DateTime.Now,
                        TotalAmount = totalAmount,
                        StatusId = 1,
                        ShippingAddress = address,
                        PaymentMethod = "Bank Transfer"
                    };

                    await _order.AddAsync(newOrder);
                    int orderId = newOrder.OrderId;

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
                    var selectedCartItems = cartItems
                        .Where(c => c.UserId == userID && c.ProductId.HasValue && selectedItemsList.Contains(c.ProductId.Value))
                        .ToList();

                    Console.WriteLine($"Số lượng mục trong selectedCartItems: {selectedCartItems.Count}");
                    foreach (var item in selectedCartItems)
                    {
                        await _cart.DeleteAsync(item.CartId);
                    }
                    currentUser.TotalAmount = (currentUser.TotalAmount ?? 0) + totalAmount;
                    await _user.UpdateAsync(currentUser);

                    await _hubContext.Clients.All.SendAsync("ReceiveOrderNotification", orderId, userName, totalAmount);
                }
            }
            finally
            {
                semaphore.Release();
            }

            if (!canProceedWithOrder)
            {
                TempData["ErrorMessage"] = errorMessage;
                return RedirectToPage();
            }

            return RedirectToPage();
        }
        public async Task<IActionResult> OnGetCheckPayment(string paymentCode, string address, string selectedItems)
        {
            try
            {               
                int[] selectedItemIds = selectedItems.Split(',').Select(int.Parse).ToArray();
                await OnPostPayment(address, selectedItemIds);
                return new JsonResult(new { success = true, message = "Thanh toán thành công!" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Lỗi hệ thống: {ex.Message}" });
            }
        }

        public async Task<IActionResult> OnGetSendConfirmationEmail(string address, string selectedItems)
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

                if (string.IsNullOrEmpty(selectedItems))
                {
                    return new JsonResult(new { success = false, message = "Danh sách sản phẩm được chọn không hợp lệ" });
                }

                int[] selectedItemIds = selectedItems.Split(',').Select(int.Parse).ToArray();
                var selectedItemIdsList = selectedItemIds.ToList();

                var cartDetails = (from cart in cartItems
                                   join product in products on cart.ProductId equals product.ProductId
                                   where cart.UserId == userId && cart.ProductId.HasValue && selectedItemIdsList.Contains(cart.ProductId.Value)
                                   select new CartViewModel
                                   {
                                       ProductName = product.Title,
                                       Quantity = cart.Quantity,
                                       Price = product.Price
                                   }).ToList();

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