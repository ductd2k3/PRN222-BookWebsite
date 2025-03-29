
namespace PRN222_Final_Project.ModelDto
{
    public class OrderStatistic
    {
        public decimal TotalAmount { get; set; }
        public int TotalOrder {  get; set; }
        public int CompletedOrder { get; set; }
        public int ProcessedOrder { get; set; }
        public decimal AverageOrderValue { get; set; }
    }
}
