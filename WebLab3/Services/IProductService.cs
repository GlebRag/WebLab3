using WebLab3.Data.Models;
using WebLab3.Models.Product;

namespace WebLab3.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductData>> GetAllProductsAsync(string sort, string dir);
        Task CreateProductAsync(ProductCreationViewModel viewModel);
        Task DeleteProductAsync(int productId);
        Task<ProductData> GetProductByIdAsync(int productId);
        Task UpdateProductAsync(ProductEditViewModel viewModel);
        Task<IEnumerable<ProductData>> SearchProductsAsync(string name, string manufacturer, string barcode, decimal? purchasePrice, int? count);
        Task Create50RandomProductsAsync();
    }
}
