using System.ComponentModel.DataAnnotations;

namespace PointOfSalesWebApplication.Models
{
    public enum SaleStatus 
    {
        Devis,
        Paid,
        Canceled
    }

    public class Sale 
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime SaleDate { get; set; } = DateTime.Today;

        public int? ClientID { get; set; }
        public Person? Client { get; set; }

        public List<SaleLine> Lines { get; set; } = new();
        public double TotalCost { get; set; }

        public SaleStatus Status { get; set; } = SaleStatus.Devis;
    }
}