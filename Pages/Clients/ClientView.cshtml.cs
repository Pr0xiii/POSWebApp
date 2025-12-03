using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PointOfSalesWebApplication.Models;
using PointOfSalesWebApplication.Services;

namespace PointOfSalesWebApplication.Pages
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

        public IActionResult OnGet(int? clientID)
        {
            if (clientID == null)
            {
                Client = new Person();
                Client.ID = _clientService.GetRandomID();
                return Page();
            }

            Client = _clientService.GetClientById(clientID);
            if (Client == null)
            {
                return RedirectToPage("/Clients/Clients");
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();
            if (Client == null) return Page();

            _clientService.UpdateClient(Client);
            return RedirectToPage("/Clients/Clients");
        }
    }
}
