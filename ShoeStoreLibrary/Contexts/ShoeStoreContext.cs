using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ShoeStoreLibrary.Models;

namespace ShoeStoreLibrary.Contexts;

public partial class ShoeStoreContext : DbContext
{
    public ShoeStoreContext()
    {
    }

    public ShoeStoreContext(DbContextOptions<ShoeStoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<FullName> FullNames { get; set; }

    public virtual DbSet<Manufacturer> Manufacturers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductNames> ProductNames { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<UnitOfMeasure> UnitOfMeasures { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRoles> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-LTJ7OE7\\SQLEXPRESS;Database=ShoeStore;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");

            entity.Property(e => e.CategoryName).HasMaxLength(100);
        });

        modelBuilder.Entity<FullName>(entity =>
        {
            entity.ToTable("FullName");

            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Patronymic).HasMaxLength(50);
        });

        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.ToTable("Manufacturer");

            entity.Property(e => e.ManufacturerName).HasMaxLength(255);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Order");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
        .HasForeignKey(d => d.UserId) 
        .HasConstraintName("FK_Order_User");

            entity.HasOne(d => d.OrderStatus).WithMany(p => p.Orders)
                .HasForeignKey(d => d.OrderStatusId)
                .HasConstraintName("FK_Order_OrderStatus");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.ToTable("OrderDetail");

            entity.Property(e => e.ProductId).HasMaxLength(50);

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_OrderDetail_Order");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_OrderDetail_Product");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.ToTable("OrderStatus");

            entity.Property(e => e.OrderStatusName)
                .HasMaxLength(50)
                .HasDefaultValue("Новый");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.Property(e => e.ProductId).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ProductDescription).HasMaxLength(255);

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_Product_Category");

            entity.HasOne(d => d.Manufacturer).WithMany(p => p.Products)
                .HasForeignKey(d => d.ManufacturerId)
                .HasConstraintName("FK_Product_Manufacturer");

            entity.HasOne(d => d.ProductName).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProductNameId)
                .HasConstraintName("FK_Product_ProductName");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Products)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("FK_Product_Supplier");

            entity.HasOne(d => d.UnitOfMeasure).WithMany(p => p.Products)
                .HasForeignKey(d => d.UnitOfMeasureId)
                .HasConstraintName("FK_Product_UnitOfMeasure");
        });

        modelBuilder.Entity<ProductNames>(entity =>
        {
            entity.HasKey(e => e.ProductNameId).HasName("PK_ProductName");

            entity.Property(e => e.ProductName)
                .HasMaxLength(100)
                .HasColumnName("ProductName");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.ToTable("Supplier");

            entity.Property(e => e.SupplierName).HasMaxLength(50);
        });

        modelBuilder.Entity<UnitOfMeasure>(entity =>
        {
            entity.ToTable("UnitOfMeasure");

            entity.Property(e => e.UnitName).HasMaxLength(20);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.Login).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(100);

            entity.HasOne(d => d.FullName).WithMany(p => p.Users)
                .HasForeignKey(d => d.FullNameId)
                .HasConstraintName("FK_User_FullName");

            //entity.HasMany(d => d.Roles).WithMany(p => p.Users)
            //    .UsingEntity<Dictionary<string, object>>(
            //        "UserRole",
            //        r => r.HasOne<Role>().WithMany()
            //            .HasForeignKey("RoleId")
            //            .OnDelete(DeleteBehavior.ClientSetNull)
            //            .HasConstraintName("FK__UserRoles__RoleI__03F0984C"),
            //        l => l.HasOne<User>().WithMany()
            //            .HasForeignKey("UserId")
            //            .OnDelete(DeleteBehavior.ClientSetNull)
            //            .HasConstraintName("FK__UserRoles__UserI__02FC7413"),
            //        j =>
            //        {
            //            j.HasKey("UserId", "RoleId").HasName("PK__UserRole__AF2760AD107FD7A4");
            //            j.ToTable("UserRoles");
            //        });
        });

        modelBuilder.Entity<UserRoles>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId });
            entity.ToTable("UserRoles");

            entity.HasOne(u => u.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(e => e.UserId);

            entity.HasOne(r => r.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(e => e.RoleId);
        });


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
