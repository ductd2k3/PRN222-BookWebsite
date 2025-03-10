namespace PRN222_Final_Project.ModelDto
{
    public class CartViewModel
    {
        public int? ProductId { get; set; }
        public int? UserId { get; set; }
        public int? Quantity { get; set; }
        public DateTime? AddedDate { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
    }

}
