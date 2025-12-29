using System.ComponentModel.DataAnnotations;

namespace PointOfSalesWebApplication.Models
{
    public class MoveTaskDto
    {
        public int TaskId { get; set; }
        public int SectionId { get; set; }
        public int Order { get; set; }
    }
    public class TaskInput 
    {
        public int ID { get; set; }


        [Required]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Required]
        public string PriorityLevel { get; set; } = string.Empty;
        public DateTime EndDate { get; set; } = DateTime.Today;
    }
    public class TaskModel
    {
        public int ID { get; set; }
        public string UserID { get; set; } = string.Empty;


        [Required]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Required]
        public string PriorityLevel { get; set; } = string.Empty;
        public DateTime EndDate { get; set; }

        public int Order { get; set; }
        public int SectionID { get; set; }
        public Section Section { get; set; } = null!;
    }
}
