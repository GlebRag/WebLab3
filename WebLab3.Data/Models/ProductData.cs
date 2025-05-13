using WebLab3.Data.Interfaces.Models;
using System.Numerics;

namespace WebLab3.Data.Models
{
    public class ProductData : BaseModel, IProductData
    {
        public string Name { get; set; }
        public string Manufacturer { get; set; }

        public string Barcode { get; set; }

        public decimal PurchasePrice { get; set; }

        public int Count { get; set; }


    }
}
