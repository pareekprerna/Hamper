using HamperStore.Core.Entities;
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
