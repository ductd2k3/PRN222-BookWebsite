using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_Final_Project.Models;
using PRN222_Final_Project.Services.Interface;
using BCrypt.Net;

namespace PRN222_Final_Project.Pages.User
{
    public class RegisterModel : PageModel
    {
        private readonly IUserService _userService;

        public RegisterModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public string ErrorMessage { get; set; }
        public class InputModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string ConfirmPassword { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Input.Password != Input.ConfirmPassword)
            {
                ErrorMessage = "Mật khẩu không khớp";
                return Page();
            }

            // Kiểm tra email đã tồn tại
            if (await _userService.IsDuplicateEmail(Input.Email, Input.Username))
            {
                ErrorMessage = "Email hoặc tên đăng nhập đã tồn tại!";
                return Page();
            }

            var user = new Models.User
            {
                Username = Input.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(Input.Password),
                FullName = Input.FullName,
                Email = Input.Email,
                RoleId = 2, // Role mặc định là 2 như trong code của bạn
                CreatedAt = DateTime.Now
            };

            await _userService.AddAsync(user);

            return RedirectToPage("/User/Login", new { message = "Đăng ký thành công! Vui lòng đăng nhập." });
        }
    }
}