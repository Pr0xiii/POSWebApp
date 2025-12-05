using PointOfSalesWebApplication.Models;

namespace PointOfSalesWebApplication.Services
{
    public interface ISaleService 
    {
        public Sale? CreateSale(int? clientID = null);
        public Sale GetSaleById(int saleID);

        public void AddProduct(int saleID, int productID, int qty = 1);
        public void RemoveProduct(int saleID, int productID, int qty = 1);
        public void UpdateQuantity(int saleID, int productID, int qty);

        public void SetClient(int saleID, int clientID);

        public void CalculateTotalCost(int saleID);

        public void FinalizeSale(int saleID, int clientID);
    }
}