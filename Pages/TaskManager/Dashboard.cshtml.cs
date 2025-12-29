using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PointOfSalesWebApplication.Models;
using PointOfSalesWebApplication.Services;

namespace PointOfSalesWebApplication.Pages.TaskManager
{
    public class DashboardModel : PageModel
    {
        private readonly ITaskService _service;
        private readonly UserManager<IdentityUser> _userManager;

        public List<Section> Sections { get; set; }

        public TaskInput? Task { get; set; } = new();
        public SectionInput? Section { get; set; } = new();

        public SelectList? Priorities = new SelectList(
            new List<string>
            {
                "Low",
                "Medium",
                "High"
            }
        );

        public DashboardModel(ITaskService service, UserManager<IdentityUser> userManager)
        {
            _service = service;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            await _service.EnsureDefaultSectionsAsync(user.Id);

            Sections = await _service.GetAllSectionsAsync(user.Id);

            return Page();
        }

        public async Task<IActionResult> OnPostSaveTaskAsync([FromForm] TaskInput task)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            if (!ModelState.IsValid || task == null)
            {
                return Page();
            }

            await _service.UpdateTaskAsync(task, user.Id);

            if (task.ID == 0)
            {
                TempData["ToastMessage"] = "Task created";
            }
            else
            {
                TempData["ToastMessage"] = "Task updated";
            }

            TempData["TaskCreated"] = true;

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCreateSectionAsync([FromForm] SectionInput section)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            if (!ModelState.IsValid || section == null)
            {
                return Page();
            }

            await _service.CreateSectionAsync(section, user.Id);

            TempData["TaskCreated"] = true;
            TempData["ToastMessage"] = "Section successfully created";

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveTaskAsync(int taskID, string taskName)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            await _service.RemoveTaskAsync(taskID, user.Id);

            TempData["TaskCreated"] = true;
            TempData["ToastMessage"] = $"Task : '{taskName}' successfully deleted";

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostMoveTaskAsync([FromBody] MoveTaskDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            await _service.MoveTaskAsync(dto.TaskId, dto.SectionId, dto.Order, user.Id);

            return new JsonResult(true);
        }

    }
}
