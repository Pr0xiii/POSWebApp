using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration.UserSecrets;
using PointOfSalesWebApplication.Models;
using PointOfSalesWebApplication.Services;
using System.Security.Claims;

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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            if (clientID == null)
            {
                Client = new Person();
                Client.UserId = userId;
                Client.ID = await _clientService.GetRandomIDAsync(userId);
            }

            Client = await _clientService.GetClientByIdAsync(clientID, userId);
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

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            Client.UserId = userId;

            await _clientService.UpdateClientAsync(Client);
            return RedirectToPage("/Clients/Clients");
        }
    }
}
