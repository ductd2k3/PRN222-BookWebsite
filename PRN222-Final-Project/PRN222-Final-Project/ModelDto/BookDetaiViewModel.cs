namespace PRN222_Final_Project.ModelDto
{
    public class BookDetaiViewModel
    {
        public int ProductId { get; set; }

        public int? CategoryId { get; set; }

        public string Title { get; set; } = null!;

        public string? Author { get; set; }

        public decimal Price { get; set; }

        public string? Description { get; set; }

        public int? Stock { get; set; }

        public string? ImageUrl { get; set; }


    }
}
