using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_Final_Project.Models;
using PRN222_Final_Project.Services.Interface;
using System.Threading.Tasks;

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
        public string Phone { get; set; } = string.Empty;

        [BindProperty]
        public string Address { get; set; } = string.Empty;

        [BindProperty]
        public string NewPassword { get; set; } = string.Empty;

        [BindProperty]
        public string ConfirmPassword { get; set; } = string.Empty;

        public async Task<IActionResult> OnGet()
        {
            string userIdString = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToPage("/User/Login");
            }

            int userID = int.Parse(userIdString);
            UserProfile = await _userService.GetByIdAsync(userID);

            if (UserProfile == null)
            {
                return NotFound();
            }

            // Đổ dữ liệu vào form
            FullName = UserProfile.FullName;
            Email = UserProfile.Email;
            Address = UserProfile.Address;

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            string userIdString = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToPage("/User/Login");
            }

            int userID = int.Parse(userIdString);
            var userToUpdate = await _userService.GetByIdAsync(userID);

            if (userToUpdate == null)
            {
                ModelState.AddModelError(string.Empty, "Không tìm thấy người dùng!");
                return Page();
            }

            if (!string.IsNullOrEmpty(NewPassword) && NewPassword != ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không khớp!");
                return Page();
            }

            // Cập nhật thông tin
            userToUpdate.FullName = FullName;
            userToUpdate.Email = Email;
            userToUpdate.Address = Address;

            if (!string.IsNullOrEmpty(NewPassword))
            {
                userToUpdate.Password = NewPassword;
            }

            await _userService.UpdateAsync(userToUpdate);

            TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
            return RedirectToPage();
        }
    }
}
