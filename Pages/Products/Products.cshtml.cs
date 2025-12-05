using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PointOfSalesWebApplication.Models;
using PointOfSalesWebApplication.Services;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PointOfSalesWebApplication.Pages.Products
{
    public class ProductsModel : PageModel
    {
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
        
        public ProductsModel(IProductService productService) 
        {
            _productService = productService;
        }

        public IActionResult OnGet()
        {
            var products = _productService.GetAllProducts().AsQueryable();

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

            Products = products.ToList();
            return Page();
        }

        public IActionResult OnPostDelete(int productID)
        {
            _productService.DeleteProduct(productID);
            return RedirectToPage();
        }
    }
}
