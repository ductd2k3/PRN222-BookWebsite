using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PRN222_Final_Project.Models;

public partial class BookStoreDbOptimizedContext : DbContext
{
    public BookStoreDbOptimizedContext()
    {
    }

    public BookStoreDbOptimizedContext(DbContextOptions<BookStoreDbOptimizedContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=BookStoreDB_Optimized;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__Carts__51BCD797BDC42283");

            entity.Property(e => e.CartId).HasColumnName("CartID");
            entity.Property(e => e.AddedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Product).WithMany(p => p.Carts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Carts__ProductID__5812160E");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Carts__UserID__571DF1D5");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A2B121F0506");

            entity.HasIndex(e => e.CategoryName, "UQ__Categori__8517B2E07B0FBFB1").IsUnique();

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(200);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BAF1707A2B5");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.ShippingAddress).HasMaxLength(200);
            entity.Property(e => e.StatusId)
                .HasDefaultValue(1)
                .HasColumnName("StatusID");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Status).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Orders__StatusID__4D94879B");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Orders__UserID__4CA06362");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__D3B9D30CB13A8360");

            entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__OrderDeta__Order__5165187F");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__OrderDeta__Produ__52593CB8");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__OrderSta__C8EE2043FA3BC6A6");

            entity.HasIndex(e => e.StatusName, "UQ__OrderSta__05E7698A5D7ECA26").IsUnique();

            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.StatusName).HasMaxLength(20);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6ED386B57D7");

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Author).HasMaxLength(100);
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(200)
                .HasColumnName("ImageURL");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Stock).HasDefaultValue(0);
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Products__Catego__44FF419A");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A15A60B44");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B6160CD98008B").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.RoleName).HasMaxLength(20);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACEA6D2291");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E48384A31F").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105345709DF58").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__Users__RoleID__3D5E1FD2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
