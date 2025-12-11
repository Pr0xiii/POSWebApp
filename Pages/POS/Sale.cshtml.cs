using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PointOfSalesWebApplication.Models;
using PointOfSalesWebApplication.Models.DTO;
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
        private readonly UserManager<IdentityUser> _userManager;

        private const string SessionKey = "CurrentSale";
        public SaleDto CurrentSale { get; set; }
        public List<Product> Products { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? ClientString { get; set; }
        public SelectList? ClientOptions { get; set; }

        public SaleModel(
            ISaleService saleService,
            IProductService productService,
            IClientService clientService,
            UserManager<IdentityUser> userManager)
        {
            _saleService = saleService;
            _productService = productService;
            _clientService = clientService;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            var userId = user.Id;
            Products = await _productService.GetAllProductsToSoldAsync(userId);

            var clients = await _clientService.GetAllClientsAsync(userId);
            ClientOptions = new SelectList(clients.Select(c => c.Name));

            // Load from session
            CurrentSale = HttpContext.Session.GetObject<SaleDto>(SessionKey);

            if (CurrentSale == null)
            {
                CurrentSale = new SaleDto
                {
                    Name = await _saleService.GenerateSaleNameAsync(),
                    SaleDate = DateTime.Now
                };
                HttpContext.Session.SetObject(SessionKey, CurrentSale);
            }

            if (!string.IsNullOrWhiteSpace(ClientString))
            {
                var client = clients.First(x => x.Name == ClientString);
                CurrentSale.ClientID = client.ID;
                CurrentSale.ClientName = client.Name;
                CurrentSale.ClientAddress = client.Address;

                HttpContext.Session.SetObject(SessionKey, CurrentSale);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAddProductAsync(int productID, int qty = 1)
        {
            CurrentSale = HttpContext.Session.GetObject<SaleDto>(SessionKey);

            var userId = _userManager.GetUserId(User);

            await _saleService.AddProductDtoAsync(CurrentSale, productID, userId, qty);

            HttpContext.Session.SetObject(SessionKey, CurrentSale);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveProductAsync(int productID, int qty = 1)
        {
            CurrentSale = HttpContext.Session.GetObject<SaleDto>(SessionKey);

            var userId = _userManager.GetUserId(User);

            await _saleService.RemoveProductDtoAsync(CurrentSale, productID, userId, qty);

            HttpContext.Session.SetObject(SessionKey, CurrentSale);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostFinalizeSaleAsync()
        {
            CurrentSale = HttpContext.Session.GetObject<SaleDto>(SessionKey);
            var userId = _userManager.GetUserId(User);

            await _saleService.FinalizeSaleFromDtoAsync(CurrentSale, userId);

            HttpContext.Session.Remove(SessionKey);

            TempData["ShowPaymentModal"] = true;
            return RedirectToPage();
        }
    }

}