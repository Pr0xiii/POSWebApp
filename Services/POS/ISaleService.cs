using PointOfSalesWebApplication.Models;

namespace PointOfSalesWebApplication.Services
{
    public interface ISaleService 
    {
        public int GenerateSaleID();
        public string GenerateSaleName(int id);
        public List<Product> GetAllSaleProducts();
        public Client SetSaleClient(int id);
        public void UpdateSale(Product product);
        public void CalculateSalePrice();
        public void GenerateSale();
    }
}