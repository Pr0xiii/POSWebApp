using PointOfSalesWebApplication.Data;
using PointOfSalesWebApplication.Models;

namespace PointOfSalesWebApplication.Services
{
    public class ProductService : IProductService
    {
        private readonly PosContext _context;
        private readonly Random _rand = new Random();

        private static List<Product> _products = new()
        {
            new Product
            {
                ID = 0,
                Name = "Apple",
                CostPrice = 0.53,
                SalePrice = 1.20,
                CanBeSold = true
            },
            new Product
            {
                ID = 1,
                Name = "Pizza",
                CostPrice = 3.145,
                SalePrice = 5.99,
                CanBeSold = false
            }
        };

        public ProductService(PosContext productContext)
        {
            _context = productContext;
        }

        public void DeleteProduct(int id)
        {
            Product? product = GetProductById(id);
            if (product != null)
                _context.Products.Remove(product);
            _context.SaveChanges();
        }

        public List<Product> GetAllProducts()
        {
            return _context.Products.ToList();
        }

        public Product? GetProductById(int? id)
        {
            return GetAllProducts().FirstOrDefault(x => x.ID == id);
        }

        public void UpdateProduct(Product product)
        {
            var existing = GetAllProducts().FirstOrDefault(p => p.ID == product.ID);
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

            _context.SaveChanges();
        }

        public int GetRandomID()
        {
            int id;

            do
            {
                id = _rand.Next(1000, 10000); // 1000–9999
            }
            while (GetAllProducts().Any(s => s.ID == id));

            return id;
        }
    }
}
