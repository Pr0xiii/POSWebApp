using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PointOfSalesWebApplication.Models;
using PointOfSalesWebApplication.Services;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PointOfSalesWebApplication.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace PointOfSalesWebApplication.Pages.Clients
{
    public class ClientsModel : PageModel 
    {
        private readonly IClientService _clientService;
        private readonly PosContext _context;
        private readonly UserManager<IdentityUser> _userManager;

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

        public ClientsModel(PosContext context, IClientService clientService, UserManager<IdentityUser> userManager) 
        {
            _context = context;
            _clientService = clientService;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync() 
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            var userId = user.Id;

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
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            var userId = user.Id;

            await _clientService.DeleteClientAsync(clientID, userId);
            return RedirectToPage();
        }
    }
}