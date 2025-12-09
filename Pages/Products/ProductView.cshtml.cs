using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PointOfSalesWebApplication.Models;
using PointOfSalesWebApplication.Services;

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
            if (productID == null)
            {
                Product = new Product();
                Product.ID = await _productService.GetRandomIDAsync();
                return Page();
            }

            Product = await _productService.GetProductByIdAsync(productID);
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

            await _productService.UpdateProductAsync(Product);
            return RedirectToPage("/Products/Products");
        }
    }
}
