using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_Final_Project.Models;
using PRN222_Final_Project.Services.Interface;

namespace PRN222_Final_Project.Pages.Manager
{
    public class Admin_ManageUserModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IGenericService<Role> _genericService;

        public Admin_ManageUserModel(IUserService userService, IGenericService<Role> genericService, IEnumerable<Models.User> users)
        {
            _userService = userService;
            _genericService = genericService;
        }
        public IEnumerable<Models.User> Users { get; private set; }
        public IEnumerable<Role> Roles { get; private set; }
        public int CurrentPage { get; private set; } = 1;
        [BindProperty(SupportsGet = true)] public string? Search { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; } = 5;
        public string AlertMessage { get; set; }
        public async Task<IActionResult> OnGetAsync(int pageNumber = 1)
        {
            try
            {
                Roles = await _genericService.GetAllAsync();

                CurrentPage = Math.Max(1, pageNumber);
                var (users, totalCount) = await _userService.GetUserPagesAsync(Search, CurrentPage, PageSize);
                Users = users;
                TotalCount = totalCount;
            }
            catch (Exception ex)
            {

            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(IFormCollection form)
        {
            try
            {
                int.TryParse(form["userRole"], out int roleId);
                int.TryParse(form["userId"], out int userID);
                Models.User user = new Models.User
                {
                    Username = form["userName"],
                    Email = form["userEmail"],
                    Address = form["userAddress"],
                    RoleId = roleId,
                    Password = form["userPassword"]
                };

                if (userID > 0)
                {
                    user.UserId = userID;
                    await _userService.UpdateAsync(user);
                    AlertMessage = "Chỉnh sửa người dùng thành công";
                }
                else
                {
                    await _userService.AddAsync(user);
                    AlertMessage = "Thêm người dùng thành công";
                }
            }
            catch (Exception e)
            {
                AlertMessage = e.Message;
                await OnGetAsync(int.Parse(form["crrpage"]));
                return Page();
            }
            await OnGetAsync(int.Parse(form["crrpage"]));
            return Page();
        }
    }
}
