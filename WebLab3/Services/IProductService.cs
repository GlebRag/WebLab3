using WebLab3.Data.Models;
using WebLab3.Models.Product;

namespace WebLab3.Services
{
    public interface IProductService
    {
        Task<List<ProductViewModel>> GetAllProductsAsync(string sort, string dir);
        Task CreateProductAsync(ProductCreationViewModel viewModel);
        Task DeleteProductAsync(int productId);
        Task<ProductData> GetProductByIdAsync(int productId);
        Task UpdateProductAsync(ProductEditViewModel viewModel);
        Task<List<ProductViewModel>> SearchProductsAsync(string name, string manufacturer, string barcode, string purchasePrice, string totalPrice, int count);
        Task Create50RandomProductsAsync();
    }
}
