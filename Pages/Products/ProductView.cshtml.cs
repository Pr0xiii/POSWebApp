using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PointOfSalesWebApplication.Models;
using PointOfSalesWebApplication.Services;
using System.Security.Claims;

namespace PointOfSalesWebApplication.Pages.Products
{
    public class ProductViewModel : PageModel
    {
        private readonly IProductService _productService;

        [BindProperty]
        public Product? Product { get; set; }

        public ProductViewModel(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> OnGetAsync(int? productID)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return RedirectToPage("/Account/Login");

            if (productID == null)
            {
                Product = new Product();
                Product.UserId = userId;
                Product.ID = await _productService.GetRandomIDAsync(userId);
                return Page();
            }

            Product = await _productService.GetProductByIdAsync(productID, userId);
            if (Product == null)
            {
                return RedirectToPage("/Products/Products");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            if (Product == null) return Page();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return RedirectToPage("/Account/Login");

            await _productService.UpdateProductAsync(Product, userId);
            return RedirectToPage("/Products/Products");
        }
    }
}
