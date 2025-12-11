namespace PointOfSalesWebApplication.Models.DTO
{
    public class SaleDto
    {
        //public string UserId { get; set; }
        //public int? ID { get; set; }

        public string Name { get; set; } = "";
        public DateTime SaleDate { get; set; } = DateTime.Now;

        public int? ClientID { get; set; }
        public string? ClientName { get; set; }
        public string? ClientAddress { get; set; }

        public List<SaleLineDto> Lines { get; set; } = new();

        public decimal TotalCost =>
            Math.Round(Lines.Sum(x => x.UnitPrice * x.Quantity), 2);
    }

}
