using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PointOfSalesWebApplication.Models;
using PointOfSalesWebApplication.Services;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PointOfSalesWebApplication.Data;

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
            var _clients = from c in _context.Clients
                        select c;

            if (_clients == null) return Page();

            if(!string.IsNullOrWhiteSpace(SearchString)) 
            {
                _clients = _clients.Where(x => x.Name.ToLower().Contains(SearchString.ToLower()));
            }

            _clients = SortString switch 
            {
                "NameDesc" => _clients.OrderByDescending(x => x.Name),
                _ => _clients.OrderBy(x => x.Name)
            };

            Clients = _clients.ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int clientID)
        {
            await _clientService.DeleteClientAsync(clientID);
            return RedirectToPage();
        }
    }
}