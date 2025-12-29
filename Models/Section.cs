using System.ComponentModel.DataAnnotations;

namespace PointOfSalesWebApplication.Models
{
    public class SectionInput
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
    }
    public class Section
    {
        public int ID { get; set; }
        public string UserID { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }

        public ICollection<TaskModel> Tasks { get; set; } = new List<TaskModel>();
    }
}
