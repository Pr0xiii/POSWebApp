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

        public IActionResult OnGet(int? productID)
        {
            if (productID == null)
            {
                Product = new Product();
                Product.ID = _productService.GetRandomID();
                return Page();
            }

            Product = _productService.GetProductById(productID);
            if (Product == null)
            {
                return RedirectToPage("/Products/Products");
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();
            if (Product == null) return Page();

            Console.WriteLine(Product.ID);
            _productService.UpdateProduct(Product);
            return RedirectToPage("/Products/Products");
        }
    }
}
