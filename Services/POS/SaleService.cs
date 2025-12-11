using Microsoft.EntityFrameworkCore;
using PointOfSalesWebApplication.Data;
using PointOfSalesWebApplication.Models;
using System.Runtime.CompilerServices;

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

        public async Task<Sale?> CreateSaleAsync(string userid, int? clientID = null) 
        {
            var newSale = new Sale
            {
                UserId = userid,
                Name = await GenerateSaleNameAsync(),
                ClientID = clientID,
                Client = await _clientService.GetClientByIdAsync(clientID, userid)
            };
            
            _context.Sales.Add(newSale);
            await _context.SaveChangesAsync();

            return newSale;
        }
        public async Task<Sale> GetSaleByIdAsync(int saleID, string userid) 
        {
            return await _context.Sales
                .Where(s => s.UserId == userid)
                .Include(s => s.Lines)
                .ThenInclude(l => l.Product)
                .FirstOrDefaultAsync(s => s.ID == saleID);
        }

        public async Task AddProductAsync(int saleID, int productID, string userid, int qty = 1)
        {
            var sale = await GetSaleByIdAsync(saleID, userid);
            var product = await _productService.GetProductByIdAsync(productID, userid);

            var line = sale.Lines.FirstOrDefault(x => x.ProductID == productID);
            if(line == null) 
            {
                sale.Lines.Add(new SaleLine 
                {
                    UserId = userid,
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

            await CalculateTotalCostAsync(saleID, userid);
            await _context.SaveChangesAsync();
        }
        public async Task RemoveProductAsync(int saleID, int productID, string userid, int qty = 1) 
        {
            var sale = await GetSaleByIdAsync(saleID, userid);
            var product = await _productService.GetProductByIdAsync(productID, userid);

            var line = sale.Lines
                .Where(x => x.UserId == userid)
                .FirstOrDefault(x => x.ProductID == productID);

            if(line == null) return;

            if(line.Quantity == qty) 
            {
                sale.Lines.Remove(line);
            }
            else 
            {
                line.Quantity -= qty;
            }

            await CalculateTotalCostAsync(saleID, userid);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateQuantityAsync(int saleID, int productID, int qty, string userid) 
        {
            var sale = await GetSaleByIdAsync(saleID, userid);
            var product = await _productService.GetProductByIdAsync(productID, userid);

            var line = sale.Lines
                .Where(x => x.UserId == userid)
                .FirstOrDefault(x => x.ProductID == productID);

            if (line != null) {
                line.Quantity += qty;
                await _context.SaveChangesAsync();
            }

            await CalculateTotalCostAsync(saleID, userid);
        }

        public async Task SetClientAsync(int saleID, int clientID, string userid) 
        {
            var sale = await GetSaleByIdAsync(saleID, userid);
            var client = await _clientService.GetClientByIdAsync(clientID, userid);

            sale.ClientID = clientID;
            sale.Client = client;

            await _context.SaveChangesAsync();
        }

        public async Task CalculateTotalCostAsync(int saleID, string userid) 
        {
            var sale = await GetSaleByIdAsync(saleID, userid);
            sale.TotalCost = Math.Round(sale.Lines.Sum(x => x.TotalPrice), 2);
        }

        public async Task FinalizeSaleAsync(int saleID, int clientID, string userid)
        {
            var sale = await GetSaleByIdAsync(saleID, userid);
            var client = await _clientService.GetClientByIdAsync(clientID, userid);
            sale.Status = SaleStatus.Paid;
            
            if(client != null) 
            {
                Console.WriteLine("LE NOM" + sale.Client.Name);
                await SetClientAsync(saleID, clientID, userid);
                await _clientService.AddSaleAsync(clientID, sale, userid);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<string> GenerateSaleNameAsync()
        {
            int id;
            bool exists;

            do
            {
                id = _rand.Next(1000, 10000); // 1000–9999
                exists = await _context.Clients
                    .AnyAsync(c => c.ID == id);
            }
            while (exists);

            return $"SO{DateTime.Today:yyMM}{id}";
        }

        public async Task CancelSaleAsync(int saleID, string userid) 
        {
            var sale = await GetSaleByIdAsync(saleID, userid);
            sale.Status = SaleStatus.Canceled;
            await _context.SaveChangesAsync();
        }
    }
}