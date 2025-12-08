using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PointOfSalesWebApplication.Models;
using PointOfSalesWebApplication.Services;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        public IActionResult OnGet()
        {
            var clients = _clientService.GetAllClients().AsQueryable();
            ClientOptions = new SelectList(clients.Select(x => x.Name).ToList());

            Products = _productService.GetAllProducts()
                .Where(x => x.CanBeSold)
                .ToList();

            if (SaleID.HasValue)
            {
                CurrentSale = _saleService.GetSaleById(SaleID.Value);
                if (CurrentSale == null)
                    return NotFound();
            }
            else
            {
                CurrentSale = _saleService.CreateSale();
                SaleID = CurrentSale.ID;
            }

            if (!string.IsNullOrWhiteSpace(ClientString))
            {
                var client = clients.First(x => x.Name == ClientString).ID;
                SetClientToSale(client);
            }

            _saleService.CalculateTotalCost(CurrentSale.ID);
            return Page();
        }

        public IActionResult OnPostAddProduct(int saleID, int productID, int qty = 1) 
        {   
            _saleService.AddProduct(saleID, productID, qty);
            return RedirectToPage(new { saleID });
        }
        public void RemoveProductFromSale(int productID, int qty = 1) 
        {
            if(CurrentSale == null) return;
            _saleService.RemoveProduct(CurrentSale.ID, productID, qty);
        }
        public void SetClientToSale(int clientID) 
        {
            if(CurrentSale == null) return;
            _saleService.SetClient(CurrentSale.ID, clientID);
        }

        public IActionResult OnPostFinalizeSale(int saleID, int clientID) 
        {
            _saleService.FinalizeSale(saleID, clientID);
            TempData["ShowPaymentModal"] = true;
            return RedirectToPage();
        }
    }
}