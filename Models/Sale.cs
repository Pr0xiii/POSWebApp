using System.ComponentModel.DataAnnotations;

namespace PointOfSalesWebApplication.Models
{
    public int ID { get; set; }
    public string SaleName { get; set; }
    public DateTime SaleDate { get; set; }
    public Person? Client { get; set; }
    public List<Produt> Products { get; set; }
    public double SalePrice { get; set; }
}