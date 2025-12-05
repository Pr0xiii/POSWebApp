using System.ComponentModel.DataAnnotations;

namespace PointOfSalesWebApplication.Models
{
    public class SaleLine
    {
        public int ProductID { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }

        public double TotalPrice => UnitPrice * Quantity;
    }
}