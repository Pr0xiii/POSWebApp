using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.Elfie.Model;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using PointOfSalesWebApplication.Models;
using PointOfSalesWebApplication.Services;
using System.Security.Claims;

namespace PointOfSalesWebApplication.Pages.Products
{
    public class ProductViewModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty]
        public Product? Product { get; set; }

        public ProductViewModel(IProductService productService, UserManager<IdentityUser> userManager)
        {
            _productService = productService;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(int? productID)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            var userId = user.Id;

            if (productID == null)
            {
                Product = new Product();
                Product.UserId = userId;
            }
            else
            {
                Product = await _productService.GetProductByIdAsync(productID, userId);
                if (Product == null)
                {
                    return RedirectToPage("/Products/Products");
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            if (Product == null) return Page();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            var userId = user.Id;

            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //if (userId == null)
            //    return RedirectToPage("/Account/Login", new { area = "Identity" });

            //Product.UserId = userId;

            await _productService.UpdateProductAsync(Product, userId);
            return RedirectToPage("/Products/Products");
        }
    }
}
