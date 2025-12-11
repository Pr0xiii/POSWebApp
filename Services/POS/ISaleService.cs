using PointOfSalesWebApplication.Models;
using PointOfSalesWebApplication.Models.DTO;

namespace PointOfSalesWebApplication.Services
{
    public interface ISaleService 
    {
        Task<Sale?> CreateSaleAsync(string userid, int? clientID = null);
        Task<Sale> GetSaleByIdAsync(int saleID, string userid);

        Task AddProductDtoAsync(SaleDto sale, int productID, string userid, int qty = 1);
        Task RemoveProductDtoAsync(SaleDto sale, int productID, string userid, int qty = 1);
        Task UpdateQuantityDtoAsync(SaleDto sale, int productID, int qty, string userid);

        Task SetClientAsync(Sale sale, int? clientID, string userid);

        Task CalculateTotalCostAsync(Sale sale, string userid);

        Task FinalizeSaleFromDtoAsync(SaleDto sale, string userid);

        Task CancelSaleAsync(int saleID, string userid);

        Task<string> GenerateSaleNameAsync();
    }
}