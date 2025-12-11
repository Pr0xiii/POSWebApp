using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PointOfSalesWebApplication.Data;
using PointOfSalesWebApplication.Models;
using PointOfSalesWebApplication.Services;
using System.Linq;
using System.Security.Claims;

namespace PointOfSalesWebApplication.Pages.POS
{
    public class SaleGroup
    {
        public string GroupKey { get; set; }
        public List<Sale> Items { get; set; }
    }

    public class SalesViewModel : PageModel
    {
        private readonly PosContext _context;
        private readonly ISaleService _saleService;
        public List<SaleGroup> Groups { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SortString { get; set; }
        [BindProperty(SupportsGet = true)]
        public string GroupString { get; set; }

        public SelectList? SortOptions = new SelectList(
            new List<string> {
                "OrderDateDesc",
                "OrderDateAsc",
                "TotalAsc",
                "TotalDesc",
                "ClientNameAsc",
                "ClientNameDesc"
            }
        );

        public SelectList? GroupOptions = new SelectList(
            new List<string> {
                "--",
                "Status",
                "Client",
                "Date"
            }
        );

        public SalesViewModel(PosContext context, ISaleService saleService) 
        {
            _context = context;
            _saleService = saleService;
        }

        public async Task<IActionResult> OnGetAsync() 
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            IQueryable<Sale> _sales = _context.Sales
                .Where(s => s.UserId == userId)
                .Include(s => s.Client);

            if (!string.IsNullOrWhiteSpace(SearchString))
            {
                _sales = _sales.Where(s => s.Name.ToLower().Contains(SearchString.ToLower()));
            }

            _sales = SortString switch 
            {
                "ClientNameAsc" => _sales.OrderBy(s => s.Client.Name),
                "ClientNameDesc" => _sales.OrderByDescending(s => s.Client.Name),
                "OrderDateAsc" => _sales.OrderBy(s => s.SaleDate),
                "TotalAsc" => _sales.OrderBy(s => s.TotalCost),
                "TotalDesc" => _sales.OrderByDescending(s => s.TotalCost),
                _ => _sales.OrderByDescending(s => s.SaleDate)
            };

            Groups = GroupString switch
            {
                "Status" => await _sales
                    .GroupBy(s => s.Status)
                    .Select(g => new SaleGroup
                    {
                        GroupKey = g.Key.ToString(),
                        Items = g.ToList()
                    }).ToListAsync(),

                "Client" => await _sales
                    .GroupBy(s => s.Client.Name)
                    .Select(g => new SaleGroup
                    {
                        GroupKey = g.Key,
                        Items = g.ToList()
                    }).ToListAsync(),

                "Date" => await _sales
                    .GroupBy(s => s.SaleDate.Date)
                    .Select(g => new SaleGroup
                    {
                        GroupKey = g.Key.ToString("yyyy-MM-dd"),
                        Items = g.ToList()
                    }).ToListAsync(),

                _ => new List<SaleGroup>
                {
                    new SaleGroup
                    {
                        GroupKey = "All Sales",
                        Items = await _sales.ToListAsync()
                    }
                }
            };

            return Page();
        }

        public async Task<IActionResult> OnPostCancelSale(int saleID, string userid) 
        {
            await _saleService.CancelSaleAsync(saleID, userid);
            return RedirectToPage();
        }
    }
}