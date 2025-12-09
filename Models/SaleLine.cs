using System.ComponentModel.DataAnnotations;

namespace PointOfSalesWebApplication.Models
{
    public class SaleLine
    {
        public int ID { get; set; }
        public int SaleID { get; set; }
        public Sale Sale { get; set; }
        public int ProductID { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public decimal TotalPrice => UnitPrice * Quantity;
    }
}