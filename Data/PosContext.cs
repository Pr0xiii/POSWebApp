using Microsoft.EntityFrameworkCore;
using PointOfSalesWebApplication.Models;

namespace PointOfSalesWebApplication.Data
{
    public class PosContext : DbContext
    {
        public PosContext(DbContextOptions<PosContext> options)
            : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Person> Clients { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleLine> SaleLines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sale>()
                .Property(e => e.TotalCost)
                .HasConversion<double>();
                
            // Relations Sale -> SaleLines
            modelBuilder.Entity<Sale>()
                .HasMany(s => s.Lines)
                .WithOne(l => l.Sale)
                .HasForeignKey(l => l.SaleID);

            // Relation Sale -> Client
            modelBuilder.Entity<Sale>()
                .HasOne(s => s.Client)
                .WithMany(c => c.Sales)
                .HasForeignKey(s => s.ClientID);

            // Relation SaleLine -> Product
            modelBuilder.Entity<SaleLine>()
                .HasOne(l => l.Product)
                .WithMany()
                .HasForeignKey(l => l.ProductID);
        }
    }
}
