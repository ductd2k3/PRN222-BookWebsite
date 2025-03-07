using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_Final_Project.Models;
using PRN222_Final_Project.Services.Interface;

namespace PRN222_Final_Project.Pages.User
{
    public class HomeModel : PageModel
    {
        private readonly IGenericService<Category> _categoryService;

        public HomeModel(IGenericService<Category> categoryService)
        {
            _categoryService = categoryService;
        }

        public IEnumerable<Category> Categories { get; set; }
        public async Task OnGetAsync()
        {
            Categories = await _categoryService.GetAllAsync();
        }
    }
}
