using System.Numerics;

namespace WebLab3.Data.Interfaces.Models
{
    public interface IProductData : IBaseModel
    {
        public string Name { get; set; }
        public string Manufacturer { get; set; }

        public string Barcode { get; set; }

        public decimal PurchasePrice { get; set; }

        public int Count { get; set; }
    }
}
