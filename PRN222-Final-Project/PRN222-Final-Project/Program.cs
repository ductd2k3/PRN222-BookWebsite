using Microsoft.EntityFrameworkCore;
using PRN222_Final_Project.Models;
using PRN222_Final_Project.Repositories.Implementation;
using PRN222_Final_Project.Repositories.Interface;
using PRN222_Final_Project.Services.Implementation;
using PRN222_Final_Project.Services.Interface;

namespace PRN222_Final_Project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            //Add database
            builder.Services.AddDbContext<BookStoreDbOptimizedContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));

            //Add dependency injection
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));

            builder.Services.AddScoped<IUserRepository,  UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddScoped<IBookRepository, BookRepository>();
            builder.Services.AddScoped<IBookService, BookService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();

            app.MapRazorPages();

            app.MapFallbackToPage("/User/Home");

            app.Run();
        }
    }
}
