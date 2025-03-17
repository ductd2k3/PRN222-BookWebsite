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

            // Add database
            builder.Services.AddDbContext<BookStoreDbOptimizedContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")).UseLazyLoadingProxies());

            // Add dependency injection
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddScoped<IBookRepository, BookRepository>();
            builder.Services.AddScoped<IBookService, BookService>();

            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();

            builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
            builder.Services.AddScoped<IOrderDetailService, OrderDetailService>();
            // **Session Configuration**
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddHttpContextAccessor();

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
            app.MapFallbackToPage("/User/Home");
            // **Enable Session Middleware**
            app.UseSession();

            // Map Razor Pages
            app.MapRazorPages();

            // **Remove this line** (app.MapFallbackToPage("/User/Home");) nếu không cần thiết.
            // Vì Razor Pages đã tự xử lý routing.

            app.Run();
        }
    }
}

