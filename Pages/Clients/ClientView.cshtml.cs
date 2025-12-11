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
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty]
        public Person? Client { get; set; }

        public ClientViewModel(IClientService clientService, UserManager<IdentityUser> userManager)
        {
            _clientService = clientService;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(int? clientID)
        {
            var user = await _userManager.GetUserAsync(User);
            if (userId == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            var userId = user.Id;

            if (clientID == null)
            {
                Client = new Person();
                Client.UserId = userId;
            }
            else {
                Client = await _clientService.GetClientByIdAsync(clientID, userId);
                if (Client == null)
                {
                    return RedirectToPage("/Clients/Clients");
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            if (Product == null) return Page();

            await _clientService.UpdateClientAsync(Client);
            return RedirectToPage("/Clients/Clients");
        }
    }
}
