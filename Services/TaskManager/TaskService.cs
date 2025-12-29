using Microsoft.EntityFrameworkCore;
using PointOfSalesWebApplication.Data;
using PointOfSalesWebApplication.Models;

namespace PointOfSalesWebApplication.Services
{
    public class TaskService : ITaskService
    {
        private readonly PosContext _context;

        public TaskService(PosContext context)
        {
            _context = context;
        }

        public async Task EnsureDefaultSectionsAsync(string userId)
        {
            var hasSections = await _context.Sections
                .Include(s => s.Tasks.OrderBy(t => t.Order))
                .AnyAsync(s => s.UserID == userId);

            if (hasSections) return;

            var sections = new List<Section>
            {
                new() { Name = "New", Order = 1, UserID = userId },
                new() { Name = "In Progress", Order = 2, UserID = userId },
                new() { Name = "Done", Order = 3, UserID = userId },
                new() { Name = "Canceled", Order = 4, UserID = userId }
            };

            _context.Sections.AddRange(sections);
            await _context.SaveChangesAsync();
        }

        public async Task CreateSectionAsync(SectionInput section, string userID)
        {
            var existing = await _context.Sections
                .FirstOrDefaultAsync(x => x.Name == section.Name);

            if (existing != null)
                return;

            var maxOrder = await _context.Sections
                .Where(s => s.UserID == userID)
                .MaxAsync(s => (int?)s.Order) ?? -1;

            var newSection = new Section
            {
                Name = section.Name,
                UserID = userID,
                Order = maxOrder + 1
            };

            _context.Sections.Add(newSection);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Section>> GetAllSectionsAsync(string userID)
        {
            return await _context.Sections
                .Where(x => x.UserID == userID)
                .Include(s => s.Tasks.OrderBy(t => t.Order))
                .ToListAsync();
        }

        public async Task<List<TaskModel>> GetAllTasksAsync(string userID)
        {
            return await _context.Tasks
                .Where(x => x.UserID == userID)
                .ToListAsync();
        }

        public async Task<Section> GetSectionAsync(int id, string userID)
        {
            return await _context.Sections
                .Where(x => x.UserID == userID)
                .FirstOrDefaultAsync(x => x.ID == id);
        }

        public async Task<TaskModel> GetTaskAsync(int id, string userID)
        {
            return await _context.Tasks
                .Where(x => x.UserID == userID)
                .FirstOrDefaultAsync(x => x.ID == id);
        }

        public async Task RemoveTaskAsync(int id, string userID)
        {
            var _task = await GetTaskAsync(id, userID);

            if (_task != null)
            {
                _context.Tasks.Remove(_task);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateTaskAsync(TaskInput task, string userID)
        {
            var _task = await GetTaskAsync(task.ID, userID);

            if (_task != null)
            {
                _task.Name = task.Name;
                _task.PriorityLevel = task.PriorityLevel;
                _task.EndDate = task.EndDate;
                _task.Description = task.Description;
            }
            else
            {
                var newSection = await _context.Sections
                    .FirstOrDefaultAsync(s => s.Name == "New" && s.UserID == userID);

                var lastOrder = await _context.Tasks
                    .Where(t =>
                        t.UserID == userID &&
                        t.SectionID == newSection.ID)
                    .MaxAsync(t => (int?)t.Order) ?? 0;

                var newTask = new TaskModel
                {
                    Name = task.Name,
                    Description = task.Description,
                    PriorityLevel = task.PriorityLevel,
                    EndDate = task.EndDate,
                    Section = newSection,
                    SectionID = newSection.ID,
                    Order = lastOrder + 1,
                    UserID = userID
                };

                newSection.Tasks.Add(newTask);
                _context.Tasks.Add(newTask);
            }

            await _context.SaveChangesAsync();
        }

        public async Task MoveTaskAsync(int taskId, int newSectionId, int newOrder, string userId)
        {
            var task = await _context.Tasks
                .FirstAsync(t => t.ID == taskId && t.UserID == userId);

            task.SectionID = newSectionId;
            task.Order = newOrder;

            // Réordonner les autres tâches
            var tasksInSection = await _context.Tasks
                .Where(t => t.SectionID == newSectionId && t.ID != taskId)
                .OrderBy(t => t.Order)
                .ToListAsync();

            for (int i = 0; i < tasksInSection.Count; i++)
            {
                tasksInSection[i].Order = i >= newOrder ? i + 1 : i;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<TaskModel>> GetTodayTasks(string userID)
        {
            return await _context.Tasks
                .Where(x => x.UserID == userID &&
                x.Section.Name != "Done" &&
                x.Section.Name != "Canceled" &&
                x.EndDate == DateTime.Today)
                .ToListAsync();
        }

        public async Task<List<TaskModel>> GetLateTasks(string userID)
        {
            return await _context.Tasks
                .Where(x => x.UserID == userID &&
                x.Section.Name != "Done" &&
                x.Section.Name != "Canceled" &&
                x.EndDate < DateTime.Today)
                .ToListAsync();
        }

        public async Task<List<TaskModel>> GetFutureTasks(string userID)
        {
            return await _context.Tasks
                .Where(x => x.UserID == userID &&
                x.Section.Name != "Done" &&
                x.Section.Name != "Canceled" &&
                x.EndDate > DateTime.Today)
                .ToListAsync();
        }

        public async Task SetTaskStatusAsync(int id, int sectionID, string userID)
        {
            var task = await GetTaskAsync(id, userID);

            if (task == null)
                return;

            var section = await GetSectionAsync(sectionID, userID);

            var lastOrder = await _context.Tasks
                    .Where(t =>
                        t.UserID == userID &&
                        t.SectionID == sectionID)
                    .MaxAsync(t => (int?)t.Order) ?? 0;

            task.SectionID = sectionID;
            task.Section = section;
            task.Order = lastOrder + 1;

            await _context.SaveChangesAsync();
        }
    }
}
