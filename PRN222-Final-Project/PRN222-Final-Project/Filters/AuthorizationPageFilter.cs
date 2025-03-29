using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using PRN222_Final_Project.Services.Interface;

namespace PRN222_Final_Project.Filters
{
    public class AuthorizationPageFilter : IPageFilter
    {
        public void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {
            // Không cần xử lý
        }

        public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            //var httpContext = context.HttpContext;

            //// Kiểm tra đăng nhập qua session
            //var role = httpContext.Session.GetString("Role");

            //var pagePath = context.ActionDescriptor.DisplayName.ToLower();

            //// Quyền cho Admin
            //if (pagePath.Contains("admin") && role != "Admin")
            //{
            //    context.Result = new RedirectToPageResult("/Error/AccessDenied");
            //    return;
            //}

            //// Quyền cho Customer (các trang chỉ dành riêng cho Customer)
            //if ((pagePath.Contains("profile") || pagePath.Contains("cart") ||
            //     (pagePath.Contains("order") && !pagePath.Contains("staff"))) && role != "Customer")
            //{
            //    context.Result = new RedirectToPageResult("/Error/AccessDenied");
            //    return;
            //}

            //// Quyền cho Staff
            //if (pagePath.Contains("staff") && role != "Staff")
            //{
            //    context.Result = new RedirectToPageResult("/Error/AccessDenied");
            //    return;
            //}
        }

        public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
            // Không cần xử lý
        }
    }
}