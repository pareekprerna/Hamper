
import os
os.makedirs('output/HamperStore.Core/Entities', exist_ok=True)
os.makedirs('output/HamperStore.Infrastructure/Data', exist_ok=True)

entities = {
"City.cs": '''namespace HamperStore.Core.Entities
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public decimal DeliveryFee { get; set; }
        public int DeliveryDays { get; set; }

        public ICollection<Hamper> Hampers { get; set; } = new List<Hamper>();
        public ICollection<Inquiry> Inquiries { get; set; } = new List<Inquiry>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
''',
"Category.cs": '''namespace HamperStore.Core.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }

        public ICollection<Hamper> Hampers { get; set; } = new List<Hamper>();
    }
}
''',
"Hamper.cs": '''namespace HamperStore.Core.Entities
{
    public class Hamper
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal BasePrice { get; set; }
        public bool IsCustomizable { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public ICollection<HamperImage> Images { get; set; } = new List<HamperImage>();
        public ICollection<HamperItem> Items { get; set; } = new List<HamperItem>();
        public ICollection<City> AvailableCities { get; set; } = new List<City>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
''',
"HamperImage.cs": '''namespace HamperStore.Core.Entities
{
    public class HamperImage
    {
        public int Id { get; set; }
        public int HamperId { get; set; }
        public Hamper Hamper { get; set; } = null!;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        public int SortOrder { get; set; }
    }
}
''',
"HamperItem.cs": '''namespace HamperStore.Core.Entities
{
    public class HamperItem
    {
        public int Id { get; set; }
        public int? HamperId { get; set; }
        public Hamper? Hamper { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? StockNote { get; set; }

        public ICollection<InquiryItem> InquiryItems { get; set; } = new List<InquiryItem>();
    }
}
''',
"Customer.cs": '''namespace HamperStore.Core.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }

        public int? PreferredCityId { get; set; }
        public City? PreferredCity { get; set; }

        public ICollection<Inquiry> Inquiries { get; set; } = new List<Inquiry>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
''',
"Inquiry.cs": '''namespace HamperStore.Core.Entities
{
    public enum InquiryStatus { New, Contacted, Converted, Closed }

    public class Inquiry
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public int? HamperId { get; set; }
        public Hamper? Hamper { get; set; }

        public int CityId { get; set; }
        public City City { get; set; } = null!;

        public string? Occasion { get; set; }
        public decimal? Budget { get; set; }
        public string? Message { get; set; }
        public InquiryStatus Status { get; set; } = InquiryStatus.New;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<InquiryItem> InquiryItems { get; set; } = new List<InquiryItem>();
        public Order? Order { get; set; }
    }
}
''',
"InquiryItem.cs": '''namespace HamperStore.Core.Entities
{
    public class InquiryItem
    {
        public int Id { get; set; }
        public int InquiryId { get; set; }
        public Inquiry Inquiry { get; set; } = null!;

        public int HamperItemId { get; set; }
        public HamperItem HamperItem { get; set; } = null!;

        public int Quantity { get; set; } = 1;
    }
}
''',
"Order.cs": '''namespace HamperStore.Core.Entities
{
    public enum OrderStatus { Pending, Confirmed, Delivered, Cancelled }

    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public int? InquiryId { get; set; }
        public Inquiry? Inquiry { get; set; }

        public int CityId { get; set; }
        public City City { get; set; } = null!;

        public decimal TotalPrice { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public DateTime? DeliveryDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
''',
"OrderItem.cs": '''namespace HamperStore.Core.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int HamperId { get; set; }
        public Hamper Hamper { get; set; } = null!;

        public int Quantity { get; set; } = 1;
        public decimal Price { get; set; }
    }
}
'''
}

for fname, content in entities.items():
    with open(f'output/HamperStore.Core/Entities/{fname}', 'w') as f:
        f.write(content)

dbcontext = '''using HamperStore.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace HamperStore.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<City> Cities => Set<City>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Hamper> Hampers => Set<Hamper>();
        public DbSet<HamperImage> HamperImages => Set<HamperImage>();
        public DbSet<HamperItem> HamperItems => Set<HamperItem>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Inquiry> Inquiries => Set<Inquiry>();
        public DbSet<InquiryItem> InquiryItems => Set<InquiryItem>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Many-to-many: Hamper <-> City (availability by city)
            modelBuilder.Entity<Hamper>()
                .HasMany(h => h.AvailableCities)
                .WithMany(c => c.Hampers)
                .UsingEntity(j => j.ToTable("HamperCityAvailability"));

            modelBuilder.Entity<Hamper>()
                .Property(h => h.BasePrice)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<OrderItem>()
                .Property(o => o.Price)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Inquiry>()
                .Property(i => i.Budget)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<City>()
                .Property(c => c.DeliveryFee)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Inquiry)
                .WithOne(i => i.Order)
                .HasForeignKey<Order>(o => o.InquiryId);

            modelBuilder.Entity<Hamper>()
                .HasIndex(h => h.Slug)
                .IsUnique();

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Slug)
                .IsUnique();
        }
    }
}
'''
with open('output/HamperStore.Infrastructure/Data/AppDbContext.cs', 'w') as f:
    f.write(dbcontext)

import shutil
shutil.make_archive('output/HamperStore_EFCore_Scaffold', 'zip', 'output')
print("done")
