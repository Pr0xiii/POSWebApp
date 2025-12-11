using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PointOfSalesWebApplication.Models;
using PointOfSalesWebApplication.Services;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PointOfSalesWebApplication.Data;
using System.Security.Claims;

namespace PointOfSalesWebApplication.Pages.Clients
{
    public class ClientsModel : PageModel 
    {
        private readonly IClientService _clientService;
        private readonly PosContext _context;
        public List<Person>? Clients { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? SortString { get; set; }

        public SelectList? SortOptions = new SelectList(
            new List<string> {
                "NameAsc",
                "NameDesc"
            }
        );

        public ClientsModel(PosContext context, IClientService clientService) 
        {
            _context = context;
            _clientService = clientService;
        }

        public async Task<IActionResult> OnGetAsync() 
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            var _clients = _context.Clients
                           .Where(c => c.UserId == userId);

            if(!string.IsNullOrWhiteSpace(SearchString)) 
            {
                _clients = _clients.Where(c => c.Name.ToLower().Contains(SearchString.ToLower()));
            }

            _clients = SortString switch 
            {
                "NameDesc" => _clients.OrderByDescending(c => c.Name),
                _ => _clients.OrderBy(c => c.Name)
            };

            Clients = await _clients.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int clientID)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            await _clientService.DeleteClientAsync(clientID, userId);
            return RedirectToPage();
        }
    }
}