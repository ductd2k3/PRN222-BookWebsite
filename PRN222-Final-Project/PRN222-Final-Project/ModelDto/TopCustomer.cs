namespace PRN222_Final_Project.ModelDto
{
    public class TopCustomer
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public int TotalOrder {  get; set; }
        public decimal? TotalPrice { get; set; }
    }
}
