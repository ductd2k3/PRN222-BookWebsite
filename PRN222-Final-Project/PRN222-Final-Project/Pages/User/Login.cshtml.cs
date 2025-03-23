using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_Final_Project.Models;
using PRN222_Final_Project.Services.Implementation;
using PRN222_Final_Project.Services.Interface;

namespace PRN222_Final_Project.Pages.User
{
    public class LoginModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IGenericService<Role> _genericService;

        public LoginModel(IUserService userService, IGenericService<Role> genericService)
        {
            _userService = userService;
            _genericService = genericService;
        }

        public void OnGet()
        {
        }
        [BindProperty] public string Email { get; set; }
        [BindProperty] public string Password { get; set; }
        public string ErrorMessage { get; set; }
        [BindProperty(SupportsGet = true)] // Cho phép nhận từ query string
        public string Message { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userService.Login(Email, Password);
            if (user == null)
            {
                ErrorMessage = "Email hoặc mật khẩu không đúng!";
                return Page();
            }

            if (!user.RoleId.HasValue)
            {
                ErrorMessage = "Tài khoản không có quyền truy cập!";
                return Page();
            }
            // Set sesion
            HttpContext.Session.SetString("UserID", user.UserId.ToString());
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("Role", user.Role.RoleName);
            HttpContext.Session.SetString("UserEmail", user.Email);

            var role = await _genericService.GetByIdAsync(user.RoleId.Value);

            return role.RoleName switch
            {
                "Admin" => RedirectToPage("/Manager/Admin-Dashboard"),
                "Customer" => RedirectToPage("/User/Home"),
                "Staff" => RedirectToPage("/Manager/Staff-ManageBook"),
                _ => Page()
            };

        }
    }
}
