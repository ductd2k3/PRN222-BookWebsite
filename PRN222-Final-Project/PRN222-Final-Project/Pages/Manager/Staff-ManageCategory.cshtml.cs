using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_Final_Project.Models;
using PRN222_Final_Project.Services.Interface;

namespace PRN222_Final_Project.Pages.Manager
{
    public class Staff_ManageCategoryModel : PageModel
    {
        private readonly IGenericService<Category> _genericService;

        public Staff_ManageCategoryModel(IGenericService<Category> genericService)
        {
            _genericService = genericService;
        }
        public string AlertMessage { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            Categories = await _genericService.GetAllAsync();
            return Page();
        }

        [BindProperty]
        public Category Category { get; set; }
        public async Task<IActionResult> OnPostAsync(IFormCollection form)
        {
            int.TryParse(form["categoryId"], out int categoryId);
            Category category = new Category
            {
                CategoryName = form["categoryName"],
                Description = form["categoryDescription"]
            };
            try
            {
                if (categoryId == 0)
                {
                    await _genericService.AddAsync(category);
                    AlertMessage = "Thêm danh mục thành công";
                }
                else
                {
                    category.CategoryId = categoryId;
                    await _genericService.UpdateAsync(category);
                    AlertMessage = "Chỉnh sửa danh mục thành công";
                }
            }
            catch (Exception ex)
            {
                AlertMessage = ex.Message;
                return Page();
            }
            Categories = await _genericService.GetAllAsync();
            return Page();
        }
    }
}
