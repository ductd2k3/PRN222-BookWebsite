using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_Final_Project.Models;
using PRN222_Final_Project.Services.Interface;
using BCrypt.Net;

namespace PRN222_Final_Project.Pages.User
{
    public class ProfileModel : PageModel
    {
        private readonly IGenericService<Models.User> _userService;

        public ProfileModel(IGenericService<Models.User> userService)
        {
            _userService = userService;
        }

        public Models.User UserProfile { get; set; } = new Models.User();

        [BindProperty]
        public string FullName { get; set; } = string.Empty;

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Address { get; set; } = string.Empty;

        [BindProperty]
        public string NewPassword { get; set; } = string.Empty;

        [BindProperty]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            string userIdString = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToPage("/User/Login");
            }

            int userId = int.Parse(userIdString);
            UserProfile = await _userService.GetByIdAsync(userId);

            if (UserProfile == null)
            {
                return NotFound();
            }

            FullName = UserProfile.FullName;
            Email = UserProfile.Email;
            Address = UserProfile.Address;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            string userIdString = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToPage("/User/Login");
            }

            int userId = int.Parse(userIdString);
            var userToUpdate = await _userService.GetByIdAsync(userId);

            if (userToUpdate == null)
            {
                ErrorMessage = "Không tìm thấy người dùng!";
                return Page();
            }

            // Validate chỉ các trường bắt buộc
            if (string.IsNullOrEmpty(FullName))
            {
                ModelState.AddModelError("FullName", "Họ và tên là bắt buộc!");
                FullName = userToUpdate.FullName;
                Email = userToUpdate.Email;
                Address = userToUpdate.Address;
                return Page();
            }

            // Validate mật khẩu nếu có nhập
            if (!string.IsNullOrEmpty(NewPassword) || !string.IsNullOrEmpty(ConfirmPassword))
            {
                if (NewPassword != ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không khớp!");
                    FullName = userToUpdate.FullName;
                    Email = userToUpdate.Email;
                    Address = userToUpdate.Address;
                    return Page();
                }
                if (string.IsNullOrEmpty(NewPassword))
                {
                    ModelState.AddModelError("NewPassword", "Vui lòng nhập mật khẩu mới!");
                    FullName = userToUpdate.FullName;
                    Email = userToUpdate.Email;
                    Address = userToUpdate.Address;
                    return Page();
                }
                userToUpdate.Password = BCrypt.Net.BCrypt.HashPassword(NewPassword);
            }

            // Cập nhật thông tin
            userToUpdate.FullName = FullName;
            userToUpdate.Address = Address;

            await _userService.UpdateAsync(userToUpdate);

            // Cập nhật session
            HttpContext.Session.SetString("UserName", FullName);

            TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
            return RedirectToPage();
        }
    }
}