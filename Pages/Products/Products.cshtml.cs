using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PointOfSalesWebApplication.Data;
using PointOfSalesWebApplication.Models;
using PointOfSalesWebApplication.Services;
using System.Linq;
using System.Security.Claims;

namespace PointOfSalesWebApplication.Pages.Products
{
    public class ProductsModel : PageModel
    {
        private readonly PosContext _context;
        private readonly IProductService _productService;
        public List<Product>? Products { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? SortString { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? OrderString { get; set; }

        public SelectList? SortOptions = new SelectList(
            new List<string> {
                "NameAsc",
                "NameDesc",
                "PriceAsc",
                "PriceDesc"
            }
        );

        public SelectList? OrderOptions = new SelectList(
            new List<string> {
                "All",
                "Can be sold",
                "Cannot be sold"
            }
        );
        
        public ProductsModel(PosContext context, IProductService productService) 
        {
            _context = context;
            _productService = productService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return RedirectToPage("/Account/Login");

            var products = _context.Products
                            .Where(p => p.UserId == userId);

            if(!string.IsNullOrWhiteSpace(SearchString)) 
            {
                products = products.Where(c => c.Name.ToLower().Contains(SearchString.ToLower()));
            }

            products = SortString switch
            {
                "NameDesc" => products.OrderByDescending(p => p.Name),
                "PriceAsc" => products.OrderBy(p => p.SalePrice),
                "PriceDesc" => products.OrderByDescending(p => p.SalePrice),
                _ => products.OrderBy(p => p.Name) // valeur par défaut
            };

            products = OrderString switch 
            {
                "Can be sold" => products.Where(p => p.CanBeSold),
                "Cannot be sold" => products.Where(p => !p.CanBeSold),
                _ => products
            };

            Products = await products.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int productID)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return RedirectToPage("/Account/Login");

            await _productService.DeleteProductAsync(productID, userId);
            return RedirectToPage();
        }
    }
}
