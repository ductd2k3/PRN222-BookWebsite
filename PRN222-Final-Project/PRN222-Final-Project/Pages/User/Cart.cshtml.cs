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
        private readonly IGenericService<Order> _order;
        private readonly IGenericService<OrderDetail> _orderDetail;

        public CartModel(IGenericService<Cart> cart, IGenericService<Product> product, IGenericService<Order> order, IGenericService<OrderDetail> orderDetail)
        {
            _cart = cart;
            _product = product;
            _order = order;
            _orderDetail = orderDetail;
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
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToPage("/User/Login");
            }

            int userID = int.Parse(userIdString);
            var cartItems = await _cart.GetAllAsync();
            var products = await _product.GetAllAsync();

            List<CartViewModel> listCart = (from cart in cartItems
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
                ModelState.AddModelError("", "Giỏ hàng của bạn đang trống!");
                return Page();
            }

            decimal? totalAmount = listCart.Sum(x => x.Quantity * x.Price);
            Order newOrder = new Order
            {
                UserId = userID,
                OrderDate = DateTime.Now,
                TotalAmount = totalAmount ?? 0,
                StatusId = 1,
                ShippingAddress = address,
                PaymentMethod = "COD"
            };

            // Insert order
            await _order.AddAsync(newOrder);
            int orderId = newOrder.OrderId;
            //Insert OrderDetail
            foreach (var item in listCart)
            {
                OrderDetail orderDetail = new OrderDetail
                {
                    OrderId = orderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity ?? 0,
                    UnitPrice = item.Price
                };

                await _orderDetail.AddAsync(orderDetail);
                //Cập nhật số lượng tồn kho
                var product = products.FirstOrDefault(p => p.ProductId == item.ProductId);
                if (product != null)
                {
                    product.Stock -= item.Quantity ?? 0;
                    await _product.UpdateAsync(product);
                }
            }
            // Delete Cart
            foreach (var item in cartItems)
            {
                await _cart.DeleteAsync(item.CartId);
            }

            return RedirectToPage("/User/OrderSuccess");
        }
    }
}
