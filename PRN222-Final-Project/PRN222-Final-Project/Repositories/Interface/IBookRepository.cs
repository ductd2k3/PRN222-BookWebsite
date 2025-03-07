﻿using PRN222_Final_Project.Models;

namespace PRN222_Final_Project.Repositories.Interface
{
    public interface IBookRepository : IGenericRepository<Product>
    {
        Task<(IEnumerable<Product> Items, int TotalCount)> GetBooksAsync(
        string searchTerm = null,
        int? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        int pageNumber = 1,
        int pageSize = 10);
    }
}
