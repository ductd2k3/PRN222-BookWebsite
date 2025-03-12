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
        public int PageSize { get; set; } = 5; // Số sản phẩm trên mỗi trang
        public int CurrentPage { get; set; }

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

            ListCart = (from cart in cartItems
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
            CurrentPage = pageNumber;
            ListCart = ListCart.Skip((pageNumber - 1) * PageSize).Take(PageSize).ToList();

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

            if (cartItem != null)
            {
                cartItem.Quantity += change;
                if (cartItem.Quantity <= 0)
                {
                    await _cart.DeleteAsync(cartItem.CartId);
                }
                else
                {
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
