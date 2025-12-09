using PointOfSalesWebApplication.Models;

namespace PointOfSalesWebApplication.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<List<Product>> GetAllProductsToSoldAsync();
        Task<Product?> GetProductByIdAsync(int? id);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
        Task<int> GetRandomIDAsync();
    }
}
