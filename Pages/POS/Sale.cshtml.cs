using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PointOfSalesWebApplication.Models;
using PointOfSalesWebApplication.Services;
using System.Linq;
using System.Security.Claims;

namespace PointOfSalesWebApplication.Pages.POS
{
    public class SaleModel : PageModel
    {
        private readonly ISaleService _saleService;
        private readonly IProductService _productService;
        private readonly IClientService _clientService;

        [BindProperty(SupportsGet = true)]
        public int? SaleID { get; set; }
        public Sale? CurrentSale { get; set; }

        public List<Product> Products { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? ClientString { get; set; }

        public SelectList? ClientOptions { get; set; }

        public SaleModel(ISaleService saleService, IProductService productService, IClientService clientService) 
        {
            _saleService = saleService;
            _productService = productService;
            _clientService = clientService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            var clients = await _clientService.GetAllClientsAsync(userId);
            ClientOptions = new SelectList(clients.Select(x => x.Name).ToList());

            Products = await _productService.GetAllProductsToSoldAsync(userId);

            if (SaleID.HasValue)
            {
                CurrentSale = await _saleService.GetSaleByIdAsync(SaleID.Value, userId);
                if (CurrentSale == null)
                    return NotFound();
            }
            else
            {
                CurrentSale = await _saleService.CreateSaleAsync(userId);
                SaleID = CurrentSale.ID;
            }

            if (!string.IsNullOrWhiteSpace(ClientString))
            {
                var client = clients.First(x => x.Name == ClientString).ID;
                await SetClientToSaleAsync(client, userId);
            }

            await _saleService.CalculateTotalCostAsync(CurrentSale.ID, userId);
            return Page();
        }

        public async Task<IActionResult> OnPostAddProductAsync(int saleID, int productID, int qty = 1) 
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            await _saleService.AddProductAsync(saleID, productID, userId, qty);
            return RedirectToPage(new { saleID });
        }
        public async Task<IActionResult> OnPostRemoveProductAsync(int saleID, int productID, int qty = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            await _saleService.RemoveProductAsync(saleID, productID, userId, qty);
            return RedirectToPage(new { saleID });
        }
        public async Task SetClientToSaleAsync(int clientID, string userid) 
        {
            await _saleService.SetClientAsync(CurrentSale.ID, clientID, userid);
        }

        public async Task<IActionResult> OnPostFinalizeSaleAsync(int saleID, int clientID) 
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            await _saleService.FinalizeSaleAsync(saleID, clientID, userId);
            TempData["ShowPaymentModal"] = true;
            return RedirectToPage();
        }
    }
}