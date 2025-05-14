using WebLab3.Data.Models;
using WebLab3.Data.Repositories;
using WebLab3.Data;
using WebLab3.Models.Product;
using WebLab3.Data.Interfaces.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebLab3.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepositoryReal _productRepository;
        private readonly WebDbContext _webDbContext;
        private readonly ILogger<ProductService> _logger;
        private readonly Random _random;

        public ProductService(IProductRepositoryReal productRepository, WebDbContext webDbContext, ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _webDbContext = webDbContext;
            _logger = logger;
            _random = new Random();
        }

        public async Task<List<ProductViewModel>> GetAllProductsAsync(string sort, string dir)
        {
            var productData = _productRepository.GetAll();

            // Сортировка на основе переданных параметров
            switch (sort.ToLower())
            {
                case "name":
                    productData = (dir.ToLower() == "asc")
                        ? productData.OrderBy(v => v.Name)
                        : productData.OrderByDescending(v => v.Name);
                    break;
                case "manufacturer":
                    productData = (dir.ToLower() == "asc")
                        ? productData.OrderBy(v => v.Manufacturer)
                        : productData.OrderByDescending(v => v.Manufacturer);
                    break;
                case "barcode":
                    productData = (dir.ToLower() == "asc")
                        ? productData.OrderBy(v => v.Barcode)
                        : productData.OrderByDescending(v => v.Barcode);
                    break;
                case "purchaseprice":
                    productData = (dir.ToLower() == "asc")
                        ? productData.OrderBy(v => v.PurchasePrice)
                        : productData.OrderByDescending(v => v.PurchasePrice);
                    break;
                case "count":
                    productData = (dir.ToLower() == "asc")
                        ? productData.OrderBy(v => v.Count)
                        : productData.OrderByDescending(v => v.Count);
                    break;
                case "totalprice":
                    productData = (dir.ToLower() == "asc")
                        ? productData.OrderBy(v => v.PurchasePrice * v.Count)
                        : productData.OrderByDescending(v => v.PurchasePrice * v.Count);
                    break;
                default:
                    productData = productData.OrderBy(v => v.Name);
                    break;
            }

            var model = productData.Select(v => new ProductViewModel
            {
                Id = v.Id,
                Name = v.Name,
                Manufacturer = v.Manufacturer,
                Barcode = v.Barcode,
                PurchasePrice = v.PurchasePrice,
                Count = v.Count,
                TotalPrice = v.PurchasePrice * v.Count
            }).ToList();



            return await Task.FromResult(model);
        }


        public async Task CreateProductAsync(ProductCreationViewModel viewModel)
        {
            var dataProduct = new ProductData
            {
                Name = viewModel.Name,
                Manufacturer = viewModel.Manufacturer,
                Barcode = viewModel.Barcode,
                PurchasePrice = viewModel.PurchasePrice,
                Count = viewModel.Count
            };

            _productRepository.Create(dataProduct);
            _logger.LogInformation("Создан продукт: {Name}", viewModel.Name);
            await Task.CompletedTask;
        }

        public async Task DeleteProductAsync(int productId)
        {
            _productRepository.Delete(productId);
            _logger.LogInformation("Удалён продукт с Id: {ProductId}", productId);
            await Task.CompletedTask;
        }

        public async Task<ProductData> GetProductByIdAsync(int productId)
        {
            var product = _webDbContext.Products.FirstOrDefault(p => p.Id == productId);
            return await Task.FromResult(product);
        }

        public async Task UpdateProductAsync(ProductEditViewModel viewModel)
        {
            var dataProduct = new ProductData
            {
                Id = viewModel.Id,
                Name = viewModel.Name,
                Manufacturer = viewModel.Manufacturer,
                Barcode = viewModel.Barcode,
                PurchasePrice = viewModel.PurchasePrice,
                Count = viewModel.Count
            };

            _productRepository.Update(dataProduct, viewModel.Id);
            _logger.LogInformation("Обновлён продукт с Id: {ProductId}", viewModel.Id);
            await Task.CompletedTask;
        }


        public async Task Create50RandomProductsAsync()
        {
            string[] productNames = new[]
            {
                "Телевизор", "Холодильник", "Смартфон", "Ноутбук", "Кофемашина",
                "Часы", "Кондиционер", "Велосипед", "Фотоаппарат", "Игровая приставка"
            };

            string[] manufacturers = new[]
            {
                "США", "Германия", "Япония", "Китай", "Южная Корея",
                "Франция", "Италия", "Россия", "Канада", "Великобритания"
            };

            decimal priceMin = 5.00M;
            decimal priceMax = 50.00M;

            for (int i = 0; i < 50; i++)
            {
                string randomName = productNames[_random.Next(productNames.Length)];
                string randomManufacturer = manufacturers[_random.Next(manufacturers.Length)];
                string randomBarcode = _random.Next(10000000, 99999999).ToString();
                decimal randomPrice = Math.Round((decimal)(_random.NextDouble() * ((double)priceMax - (double)priceMin) + (double)priceMin), 2);
                int randomCount = _random.Next(1, 21);

                var dataProduct = new ProductData
                {
                    Name = randomName,
                    Manufacturer = randomManufacturer,
                    Barcode = randomBarcode,
                    PurchasePrice = randomPrice,
                    Count = randomCount
                };

                _productRepository.Add(dataProduct);
            }
            _logger.LogInformation("50 случайных продуктов созданы");
            await Task.CompletedTask;
        }
        public async Task<List<ProductViewModel>> SearchProductsAsync(string name, string manufacturer, string barcode, string purchasePrice, string totalPrice, int count)
        {

            var viewModels = _productRepository
                .Search(name, manufacturer, barcode, purchasePrice, totalPrice, count)
                .Select(dbBook =>
                {
                    return new ProductViewModel
                    {
                        Id = dbBook.Id,
                        Name = dbBook.Name,
                        Manufacturer = dbBook.Manufacturer,
                        Barcode = dbBook.Barcode,
                        PurchasePrice = dbBook.PurchasePrice,
                        Count = dbBook.Count,
                        TotalPrice = dbBook.PurchasePrice * dbBook.Count
                    };
                })
                .ToList();

            return await Task.FromResult(viewModels);
        }
    }
}
