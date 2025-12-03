using PointOfSalesWebApplication.Models;

namespace PointOfSalesWebApplication.Services
{
    public class SaleService : ISaleService
    {
        private static List<Sale> _sales = new List<Sale>();

        public int GenerateSaleID() 
        {
            if(_sales.Count == 0) 
            {
                return 1;
            }
            else {
                int maxID = _sales.Max(x => x.ID);
                return maxID + 1;
            }
        }

        public string GenerateSaleName(int id) 
        {
            return $"Sale-{DateTime.Today.ToString("d")}{id}"
        }
        public List<Product> GetAllSaleProducts();
        public Client SetSaleClient(int id);
        public void UpdateSale(Product product);
        public void CalculateSalePrice();
        public void GenerateSale();
    }
}