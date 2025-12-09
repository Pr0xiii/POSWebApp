using PointOfSalesWebApplication.Models;

namespace PointOfSalesWebApplication.Services
{
    public interface ISaleService 
    {
        Task<Sale?> CreateSaleAsync(int? clientID = null);
        Task<Sale> GetSaleByIdAsync(int saleID);

        Task AddProductAsync(int saleID, int productID, int qty = 1);
        Task RemoveProductAsync(int saleID, int productID, int qty = 1);
        Task UpdateQuantityAsync(int saleID, int productID, int qty);

        Task SetClientAsync(int saleID, int clientID);

        Task CalculateTotalCostAsync(int saleID);

        Task FinalizeSaleAsync(int saleID, int clientID);

        Task CancelSaleAsync(int saleID);
    }
}