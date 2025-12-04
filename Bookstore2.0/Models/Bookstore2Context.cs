using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Bookstore2._0.Models;

public partial class Bookstore2Context : DbContext
{
    public Bookstore2Context()
    {
    }

    public Bookstore2Context(DbContextOptions<Bookstore2Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<Publisher> Publishers { get; set; }

    public virtual DbSet<Store> Stores { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=Bookstore2;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.AuthorId).HasName("PK__Authors__70DAFC34688B543F");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FirstName)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Isbn13).HasName("PK__Books__E7B44C943702E007");

            entity.ToTable(tb => tb.HasTrigger("trg_UpdateBooks"));

            entity.Property(e => e.Isbn13).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Language)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PriceInSek)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("PriceInSEK");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Author).WithMany(p => p.Books)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK__Books__AuthorId__4316F928");

            entity.HasOne(d => d.Publisher).WithMany(p => p.Books)
                .HasForeignKey(d => d.PublisherId)
                .HasConstraintName("FK__Books__Publisher__440B1D61");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64D85EA7F024");

            entity.ToTable("Customer", tb => tb.HasTrigger("trg_UpdateCustomer"));

            entity.HasIndex(e => e.Email, "UQ__Customer__A9D10534B626EE97").IsUnique();

            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.PostalCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => new { e.StoreId, e.Isbn13 }).HasName("PK__Inventor__65F9B5C825850A3E");

            entity.ToTable("Inventory");

            entity.HasOne(d => d.Isbn13Navigation).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.Isbn13)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Isbn1__4E88ABD4");

            entity.HasOne(d => d.Store).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.StoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Store__4F7CD00D");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BCFFF695BC6");

            entity.ToTable(tb => tb.HasTrigger("trg_UpdateOrders"));

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.OrderDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.StatusId).HasDefaultValue(1);
            entity.Property(e => e.Total).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__Customer__6383C8BA");

            entity.HasOne(d => d.Status).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__StatusId__6477ECF3");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.Isbn13 }).HasName("PK__OrderIte__9DEB1F063D9BFB89");

            entity.ToTable(tb => tb.HasTrigger("trg_UpdateOrderTotal"));

            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Isbn13Navigation).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.Isbn13)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__Isbn1__693CA210");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__OrderItem__Order__68487DD7");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__OrderSta__C8EE2063198FDA3E");

            entity.HasIndex(e => e.Name, "UQ__OrderSta__737584F6FA836765").IsUnique();

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.HasKey(e => e.PublisherId).HasName("PK__Publishe__4C657FABC07C38D4");

            entity.HasIndex(e => e.Name, "UQ__Publishe__737584F65E0BC413").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.HasKey(e => e.StoreId).HasName("PK__Stores__3B82F101325CB6CF");

            entity.HasIndex(e => e.Name, "UQ__Stores__737584F6E9A844CC").IsUnique();

            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.City)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Country)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PostalCode)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
