
namespace WebLab3.Models.Product
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }

        public string Barcode { get; set; }

        public decimal PurchasePrice { get; set; }

        public decimal TotalPrice { get; set; }

        public int Count { get; set; }
    }
}