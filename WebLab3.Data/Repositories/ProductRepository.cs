using Microsoft.EntityFrameworkCore;
using WebLab3.Data.Interfaces.Repositories;
using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;
using WebLab3.Data.Interfaces.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Data.SqlClient;
using System.Text;
using System;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using System.Net;
using System.Globalization;
using WebLab3.Data.Models;


namespace WebLab3.Data.Repositories
{
    public interface IProductRepositoryReal : IProductRepository<ProductData>
    {

        void Create(ProductData dataProduct);
        IEnumerable<ProductData> Search(string name, string manufacturer, string barcode, string purchasePrice, string totalPrice, int count);
        void Update(ProductData dataProduct, int productId);

    }

    public class ProductRepository : BaseRepository<ProductData>, IProductRepositoryReal
    {
        public ProductRepository(WebDbContext webDbContext) : base(webDbContext)
        {
        }

        public void Create(ProductData dataProduct)
        {
            Add(dataProduct);
        }


        public IEnumerable<ProductData> Search(string name, string manufacturer, string barcode, string purchasePrice, string totalPrice, int count)
        {
            var parameters = new List<SqlParameter>();
            var sql = new StringBuilder("SELECT * FROM dbo.Products WHERE 1=1");

            if (!string.IsNullOrEmpty(name))
            {
                sql.Append(" AND Name = @Name");
                parameters.Add(new SqlParameter("@Name", name));
            }

            if (!string.IsNullOrEmpty(manufacturer))
            {
                sql.Append(" AND Manufacturer = @Manufacturer");
                parameters.Add(new SqlParameter("@Manufacturer", manufacturer));
            }

            if (!string.IsNullOrEmpty(barcode))
            {
                sql.Append(" AND Barcode = @Barcode");
                parameters.Add(new SqlParameter("@Barcode", barcode));
            }

            // Поиск по PurchasePrice (столбец типа decimal)
            if (!string.IsNullOrEmpty(purchasePrice))
            {
                decimal purchasePriceValue;
                // Для преобразования заменяем запятую на точку, чтобы использовать InvariantCulture
                if (decimal.TryParse(purchasePrice.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out purchasePriceValue))
                {
                    sql.Append(" AND PurchasePrice = @PurchasePrice");
                    parameters.Add(new SqlParameter("@PurchasePrice", purchasePriceValue));
                }
            }

            // Поиск по Count (столбец типа int)
            // Если count больше 0, считаем, что его вводили для фильтрации
            if (count > 0)
            {
                sql.Append(" AND Count = @Count");
                parameters.Add(new SqlParameter("@Count", count));
            }

            // Поиск по totalPrice, который вычисляется по формуле PurchasePrice*Count
            if (!string.IsNullOrEmpty(totalPrice))
            {
                decimal totalPriceValue;
                if (decimal.TryParse(totalPrice.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out totalPriceValue))
                {
                    sql.Append(" AND (PurchasePrice * Count) = @TotalPrice");
                    parameters.Add(new SqlParameter("@TotalPrice", totalPriceValue));
                }
            }

            var result = _webDbContext
                .Database
                .SqlQueryRaw<ProductData>(sql.ToString(), parameters.ToArray())
                .ToList();

            return result;
        }

        public void Update(ProductData dataProduct, int productId)
        {
            var product = _dbSet.First(x => x.Id == productId);
            product.Name = dataProduct.Name;
            product.Manufacturer = dataProduct.Manufacturer;
            product.Barcode = dataProduct.Barcode;
            product.PurchasePrice = dataProduct.PurchasePrice;
            product.Count = dataProduct.Count;
            _webDbContext.SaveChanges();
        }


    }
}
