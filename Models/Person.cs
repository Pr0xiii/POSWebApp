using System.ComponentModel.DataAnnotations;

namespace PointOfSalesWebApplication.Models
{
    public class Person 
    {
        [Required]
        public int ID { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Please enter a client name with enough characters or less than 50.")]
        public string? Name { get; set; }

        public string? Address { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        public string? PhoneNumber { get; set; }

        public List<Sale> Sales { get; set; }
    }
}