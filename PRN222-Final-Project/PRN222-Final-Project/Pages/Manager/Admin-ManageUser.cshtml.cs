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
    }
}
