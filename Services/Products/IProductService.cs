using PointOfSalesWebApplication.Models;

namespace PointOfSalesWebApplication.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync(string userid);
        Task<List<Product>> GetAllProductsToSoldAsync(string userid);
        Task<Product?> GetProductByIdAsync(int? id, string userid);
        Task UpdateProductAsync(Product product, string userid);
        Task DeleteProductAsync(int id, string userid);
        Task<int> GetRandomIDAsync(string userid);
    }
}
