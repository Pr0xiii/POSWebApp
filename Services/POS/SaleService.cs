using PointOfSalesWebApplication.Models;

namespace PointOfSalesWebApplication.Services
{
    public class SaleService : ISaleService
    {
        private static List<Sale> _sales = new();
        private readonly IProductService _productService;
        private readonly IClientService _clientService;
        private readonly Random _rand = new Random();

        public SaleService(IProductService productService, IClientService clientService) 
        {
            _productService = productService;
            _clientService = clientService;
        }

        public Sale? CreateSale(int? clientID = null) 
        {
            int newId = GenerateSaleId();

            var newSale = new Sale 
            {
                ID = newId,
                Name = GenerateSaleName(newId),
                ClientID = clientID,
                Client = _clientService.GetClientById(clientID)
            };
            
            _sales.Add(newSale);
            return newSale;
        }
        public Sale GetSaleById(int saleID) 
        {
            return _sales.FirstOrDefault(x => x.ID == saleID);
        }

        public void AddProduct(int saleID, int productID, int qty = 1) 
        {
            var sale = GetSaleById(saleID);
            var product = _productService.GetProductById(productID);

            var line = sale.Lines.FirstOrDefault(x => x.ProductID == productID);
            if(line == null) 
            {
                sale.Lines.Add(new SaleLine 
                {
                    ProductID = productID,
                    Product = product,
                    Quantity = qty,
                    UnitPrice = product.SalePrice
                });
            }
            else 
            {
                line.Quantity += qty;
            }

            CalculateTotalCost(saleID);
        }
        public void RemoveProduct(int saleID, int productID, int qty = 1) 
        {
            var sale = GetSaleById(saleID);
            var product = _productService.GetProductById(productID);

            var line = sale.Lines.FirstOrDefault(x => x.ProductID == productID);

            if(line == null) return;

            if(line.Quantity == qty) 
            {
                sale.Lines.Remove(line);
            }
            else 
            {
                line.Quantity -= qty;
            }

            CalculateTotalCost(saleID);
        }
        public void UpdateQuantity(int saleID, int productID, int qty) 
        {
            var sale = GetSaleById(saleID);
            var product = _productService.GetProductById(productID);

            var line = sale.Lines.FirstOrDefault(x => x.ProductID == productID);

            if(line != null) {
                line.Quantity += qty;
            }

            CalculateTotalCost(saleID);
        }

        public void SetClient(int saleID, int clientID) 
        {
            var sale = GetSaleById(saleID);
            var client = _clientService.GetClientById(clientID);

            sale.ClientID = clientID;
            sale.Client = client;
        }

        public void CalculateTotalCost(int saleID) 
        {
            var sale = GetSaleById(saleID);
            sale.TotalCost = sale.Lines.Sum(x => x.TotalPrice);
        }

        public void FinalizeSale(int saleID, int clientID) 
        {
            var sale = GetSaleById(saleID);
            var client = _clientService.GetClientById(clientID);

            if(client != null) 
            {
                _clientService.AddSale(clientID, sale);
            }

            sale.Status = SaleStatus.Paid;
        }

        private int GenerateSaleId()
        {
            int id;

            do
            {
                id = _rand.Next(1000, 10000); // 1000–9999
            }
            while (_sales.Any(s => s.ID == id));

            return id;
        }
        private string GenerateSaleName(int id) 
        {
            return $"SO{DateTime.Today:yyMM}{id}";
        }
    }
}