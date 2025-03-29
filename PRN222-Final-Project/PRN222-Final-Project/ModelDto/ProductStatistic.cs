using PRN222_Final_Project.Models;

namespace PRN222_Final_Project.ModelDto
{
    public class ProductStatistic
    {
        public int TotalBook { get; set; } // Tổng số sách bán ra
        public IEnumerable<ProductStatisticDTO> TopProductsSeller { get; set; }
        public Category BestCategory { get; set; } // Danh mục bán chạy nhất
    }
}
