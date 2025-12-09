using System.ComponentModel.DataAnnotations;

namespace PointOfSalesWebApplication.Models
{
    public class Product
    {
        public int ID { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Please enter a name with at least 2 characters and maximum 50 characters.")]
        public string Name { get; set; } = "New Product";

        [Required]
        [Range(0.001, 100000, ErrorMessage = "Please enter a valid number.")]
        public decimal CostPrice { get; set; }

        [Required]
        [Range(0.001, 100000, ErrorMessage = "Please enter a valid number.")]
        public decimal SalePrice { get; set; }
        public bool CanBeSold { get; set; }
    }
}
