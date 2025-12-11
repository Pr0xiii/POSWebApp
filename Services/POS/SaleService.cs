using Microsoft.EntityFrameworkCore;
using PointOfSalesWebApplication.Data;
using PointOfSalesWebApplication.Models;
using PointOfSalesWebApplication.Models.DTO;
using System.Runtime.CompilerServices;

namespace PointOfSalesWebApplication.Services
{
    public class SaleService : ISaleService
    {
        private readonly PosContext _context;
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

        public async Task AddProductDtoAsync(SaleDto sale, int productID, string userId, int qty)
        {
            var product = await _productService.GetProductByIdAsync(productID, userId);

            var line = sale.Lines.FirstOrDefault(l => l.ProductID == productID);

            if (line == null)
            {
                sale.Lines.Add(new SaleLineDto
                {
                    ProductID = productID,
                    ProductName = product.Name,
                    UnitPrice = product.SalePrice,
                    Quantity = qty
                });
            }
            else
            {
                line.Quantity += qty;
            }
        }

        public Task RemoveProductDtoAsync(SaleDto sale, int productID, string userId, int qty)
        {
            var line = sale.Lines.FirstOrDefault(l => l.ProductID == productID);
            if (line == null) return Task.CompletedTask;

            if (line.Quantity <= qty)
                sale.Lines.Remove(line);
            else
                line.Quantity -= qty;

            return Task.CompletedTask;
        }

        public async Task UpdateQuantityDtoAsync(SaleDto sale, int productID, int qty, string userid) 
        {
            var product = await _productService.GetProductByIdAsync(productID, userid);

            var line = sale.Lines
                .FirstOrDefault(x => x.ProductID == productID);

            if (line != null) {
                line.Quantity += qty;
            }
        }

        public async Task SetClientAsync(Sale sale, int? clientID, string userid) 
        {
            var client = await _clientService.GetClientByIdAsync(clientID, userid);

            sale.ClientID = clientID;
            sale.Client = client;
        }

        public async Task CalculateTotalCostAsync(Sale sale, string userid) 
        {
            sale.TotalCost = Math.Round(sale.Lines.Sum(x => x.TotalPrice), 2);
        }

        public async Task FinalizeSaleFromDtoAsync(SaleDto dto, string userId)
        {
            var sale = new Sale
            {
                UserId = userId,
                Name = dto.Name,
                SaleDate = dto.SaleDate,
                ClientID = dto.ClientID,
                TotalCost = dto.TotalCost,
                Status = SaleStatus.Paid,
                Lines = dto.Lines.Select(l => new SaleLine
                {
                    UserId = userId,
                    ProductID = l.ProductID,
                    Quantity = l.Quantity,
                    UnitPrice = l.UnitPrice
                }).ToList()
            };

            _context.Sales.Add(sale);
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