using PointOfSalesWebApplication.Models;

namespace PointOfSalesWebApplication.Services
{
    public interface IProductService
    {
        public List<Product> GetAllProducts();
        public Product? GetProductById(int? id);
        public void UpdateProduct(Product product);
        public void DeleteProduct(int id);
        public int GetRandomID();
    }
}
