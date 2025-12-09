using Microsoft.EntityFrameworkCore;
using PointOfSalesWebApplication.Data;
using PointOfSalesWebApplication.Models;

namespace PointOfSalesWebApplication.Services
{
    public class SaleService : ISaleService
    {
        private readonly PosContext _context;
        private static List<Sale> _sales = new();
        private readonly IProductService _productService;
        private readonly IClientService _clientService;
        private readonly Random _rand = new Random();

        public SaleService(PosContext context, IProductService productService, IClientService clientService) 
        {
            _context = context;
            _productService = productService;
            _clientService = clientService;
        }

        public async Task<Sale?> CreateSaleAsync(int? clientID = null) 
        {
            int newId = await GenerateSaleIdAsync();

            var newSale = new Sale 
            {
                ID = newId,
                Name = GenerateSaleName(newId),
                ClientID = clientID,
                Client = await _clientService.GetClientByIdAsync(clientID)
            };
            
            _context.Sales.Add(newSale);
            await _context.SaveChangesAsync();

            return newSale;
        }
        public async Task<Sale> GetSaleByIdAsync(int saleID) 
        {
            return await _context.Sales
                .Include(s => s.Lines)
                .ThenInclude(l => l.Product)
                .FirstOrDefaultAsync(s => s.ID == saleID);
        }

        public async Task AddProductAsync(int saleID, int productID, int qty = 1) 
        {
            var sale = await GetSaleByIdAsync(saleID);
            var product = await _productService.GetProductByIdAsync(productID);

            var line = sale.Lines.FirstOrDefault(x => x.ProductID == productID);
            if(line == null) 
            {
                sale.Lines.Add(new SaleLine 
                {
                    SaleID = saleID,
                    Sale = sale,
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

            await CalculateTotalCostAsync(saleID);
            await _context.SaveChangesAsync();
        }
        public async Task RemoveProductAsync(int saleID, int productID, int qty = 1) 
        {
            var sale = await GetSaleByIdAsync(saleID);
            var product = await _productService.GetProductByIdAsync(productID);

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

            await CalculateTotalCostAsync(saleID);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateQuantityAsync(int saleID, int productID, int qty) 
        {
            var sale = await GetSaleByIdAsync(saleID);
            var product = await _productService.GetProductByIdAsync(productID);

            var line = sale.Lines.FirstOrDefault(x => x.ProductID == productID);

            if(line != null) {
                line.Quantity += qty;
                await _context.SaveChangesAsync();
            }

            await CalculateTotalCostAsync(saleID);
        }

        public async Task SetClientAsync(int saleID, int clientID) 
        {
            var sale = await GetSaleByIdAsync(saleID);
            var client = await _clientService.GetClientByIdAsync(clientID);

            sale.ClientID = clientID;
            sale.Client = client;

            await _context.SaveChangesAsync();
        }

        public async Task CalculateTotalCostAsync(int saleID) 
        {
            var sale = await GetSaleByIdAsync(saleID);
            sale.TotalCost = Math.Round(sale.Lines.Sum(x => x.TotalPrice), 2);
        }

        public async Task FinalizeSaleAsync(int saleID, int clientID) 
        {
            var sale = await GetSaleByIdAsync(saleID);
            var client = await _clientService.GetClientByIdAsync(clientID);
            sale.Status = SaleStatus.Paid;
            
            if(client != null) 
            {
                Console.WriteLine("LE NOM" + sale.Client.Name);
                await SetClientAsync(saleID, clientID);
                await _clientService.AddSaleAsync(clientID, sale);
            }

            await _context.SaveChangesAsync();
        }

        private async Task<int> GenerateSaleIdAsync()
        {
            int id;
            bool exists;

            do
            {
                id = _rand.Next(1000, 10000); // 1000–9999
                exists = await _context.Clients.AnyAsync(c => c.ID == id);
            }
            while (exists);

            return id;
        }
        private string GenerateSaleName(int id) 
        {
            return $"SO{DateTime.Today:yyMM}{id}";
        }

        public async Task CancelSaleAsync(int saleID) 
        {
            var sale = await GetSaleByIdAsync(saleID);
            sale.Status = SaleStatus.Canceled;
            await _context.SaveChangesAsync();
        }
    }
}