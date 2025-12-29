using PointOfSalesWebApplication.Models;

namespace PointOfSalesWebApplication.Services
{
    public interface ITaskService
    {
        Task EnsureDefaultSectionsAsync(string userId);

        Task<List<TaskModel>> GetAllTasksAsync(string userID);
        Task<TaskModel> GetTaskAsync(int id, string userID);
        Task<List<Section>> GetAllSectionsAsync(string userID);
        Task<Section> GetSectionAsync(int id, string userID);

        Task<List<TaskModel>> GetTodayTasks(string userID);
        Task<List<TaskModel>> GetLateTasks(string userID);
        Task<List<TaskModel>> GetFutureTasks(string userID);

        Task UpdateTaskAsync(TaskInput task, string userID);
        Task RemoveTaskAsync(int id, string userID);
        Task SetTaskStatusAsync(int id, int sectionID, string userID);

        Task CreateSectionAsync(SectionInput section, string userID);
        Task MoveTaskAsync(int taskId, int newSectionId, int newOrder, string userId);
    }
}
