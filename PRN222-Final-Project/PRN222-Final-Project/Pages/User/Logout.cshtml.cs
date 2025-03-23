using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PRN222_Final_Project.Pages.User
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Xóa Session
            HttpContext.Session.Clear();

            // Redirect về trang chủ hoặc login
            return RedirectToPage("/User/Login");
        }
    }
}
