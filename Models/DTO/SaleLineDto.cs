namespace PointOfSalesWebApplication.Models.DTO
{
    public class SaleLineDto
    {
        //public string UserId { get; set; }
        //public int? ID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; } = "";
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }

        public decimal TotalPrice => UnitPrice * Quantity;
    }

}
