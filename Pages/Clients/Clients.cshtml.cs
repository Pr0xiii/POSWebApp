using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PointOfSalesWebApplication.Models;
using PointOfSalesWebApplication.Services;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace PointOfSalesWebApplication.Pages.Clients
{
    public class ClientsModel : PageModel 
    {
        private readonly IClientService _clientService;
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

        public ClientsModel(IClientService clientService) 
        {
            _clientService = clientService;
        }

        public IActionResult OnGet() 
        {
            var _clients = _clientService.GetAllClients().AsQueryable();

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

        public IActionResult OnPostDelete(int clientID)
        {
            _clientService.DeleteClient(clientID);
            return RedirectToPage();
        }
    }
}