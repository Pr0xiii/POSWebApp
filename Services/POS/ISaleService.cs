using PointOfSalesWebApplication.Models;

namespace PointOfSalesWebApplication.Services
{
    public interface ISaleService 
    {
        Task<Sale?> CreateSaleAsync(string userid, int? clientID = null);
        Task<Sale> GetSaleByIdAsync(int saleID, string userid);

        Task AddProductAsync(int saleID, int productID, string userid, int qty = 1);
        Task RemoveProductAsync(int saleID, int productID, string userid, int qty = 1);
        Task UpdateQuantityAsync(int saleID, int productID, int qty, string userid);

        Task SetClientAsync(int saleID, int clientID, string userid);

        Task CalculateTotalCostAsync(int saleID, string userid);

        Task FinalizeSaleAsync(int saleID, int clientID, string userid);

        Task CancelSaleAsync(int saleID, string userid);
    }
}