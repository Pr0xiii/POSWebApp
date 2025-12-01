using PointOfSalesWebApplication.Models;

namespace PointOfSalesWebApplication.Services
{
    public class ProductService : IProductService
    {
        private static List<Product> _products = new()
        {
            new Product
            {
                ID = 0,
                Name = "Apple",
                CostPrice = 0.53,
                SalePrice = 1.20
            },
            new Product
            {
                ID = 1,
                Name = "Pizza",
                CostPrice = 3.145,
                SalePrice = 5.99
            }
        };

        public void DeleteProduct(int id)
        {
            Product? product = GetProductById(id);
            if (product != null)
                _products.Remove(product);
        }

        public List<Product> GetAllProducts()
        {
            return _products;
        }

        public Product? GetProductById(int? id)
        {
            return _products.FirstOrDefault(x => x.ID == id);
        }

        public void UpdateProduct(Product product)
        {
            var existing = _products.FirstOrDefault(p => p.ID == product.ID);
            if (existing != null)
            {
                existing.Name = product.Name;
                existing.CostPrice = product.CostPrice;
                existing.SalePrice = product.SalePrice;
                existing.IsPublished = product.IsPublished;
            }
            else
            {
                _products.Add(product);
            }
        }

        public int GetRandomID()
        {
            int maxID = GetAllProducts().Max(x => x.ID);
            return maxID + 1;
        }
    }
}
