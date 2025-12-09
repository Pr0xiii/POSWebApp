using Microsoft.EntityFrameworkCore;
using PointOfSalesWebApplication.Data;
using PointOfSalesWebApplication.Models;

namespace PointOfSalesWebApplication.Services
{
    public class ProductService : IProductService
    {
        private readonly PosContext _context;
        private readonly Random _rand = new Random();

        public ProductService(PosContext productContext)
        {
            _context = productContext;
        }

        public async Task DeleteProductAsync(int id)
        {
            Product? product = await GetProductByIdAsync(id);
            if (product != null)
                _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<List<Product>> GetAllProductsToSoldAsync() 
        {
            return await _context.Products
                            .Where(p => p.CanBeSold)
                            .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int? id)
        {
            if (id == null) return null;

            return await _context.Products
                .FirstOrDefaultAsync(p => p.ID == id);
        }

        public async Task UpdateProductAsync(Product product)
        {
            var existing = await GetProductByIdAsync(product.ID);
            if (existing != null)
            {
                existing.Name = product.Name;
                existing.CostPrice = product.CostPrice;
                existing.SalePrice = product.SalePrice;
                existing.CanBeSold = product.CanBeSold;
            }
            else
            {
                _context.Products.Add(product);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<int> GetRandomIDAsync()
        {
            int id;
            bool exists;

            do
            {
                id = _rand.Next(1000, 10000); // 1000–9999
                exists = await _context.Clients.AnyAsync(c => c.ID == id);
            }
            while (exists);

            return id;
        }
    }
}
