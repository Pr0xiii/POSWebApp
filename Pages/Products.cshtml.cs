using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PointOfSalesWebApplication.Models;
using PointOfSalesWebApplication.Services;

namespace PointOfSalesWebApplication.Pages
{
    public class ProductsModel : PageModel
    {
        private readonly IProductService _productService;
        public List<Product> Products { get; set; }
        
        public ProductsModel(IProductService productService) 
        {
            _productService = productService;
        }

        public void OnGet()
        {
            Products = _productService.GetAllProducts();
        }

        public IActionResult OnPostDelete(int productID)
        {
            _productService.DeleteProduct(productID);
            return RedirectToPage();
        }
    }
}
