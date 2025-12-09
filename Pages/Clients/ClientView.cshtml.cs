using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PointOfSalesWebApplication.Models;
using PointOfSalesWebApplication.Services;

namespace PointOfSalesWebApplication.Pages.Clients
{
    public class ClientViewModel : PageModel
    {
        private readonly IClientService _clientService;

        [BindProperty]
        public Person? Client { get; set; }

        public ClientViewModel(IClientService clientService)
        {
            _clientService = clientService;
        }

        public async Task<IActionResult> OnGetAsync(int? clientID)
        {
            if (clientID == null)
            {
                Client = new Person();
                Client.ID = await _clientService.GetRandomIDAsync();
                return Page();
            }

            Client = await _clientService.GetClientByIdAsync(clientID);
            if (Client == null)
            {
                return RedirectToPage("/Clients/Clients");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            if (Client == null) return Page();

            await _clientService.UpdateClientAsync(Client);
            return RedirectToPage("/Clients/Clients");
        }
    }
}
