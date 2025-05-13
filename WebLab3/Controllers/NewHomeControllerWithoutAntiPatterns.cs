using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using WebLab3.Data;
using WebLab3.Data.Models;
using WebLab3.Data.Repositories;
using WebLab3.Models;
using WebLab3.Models.Product;
using WebLab3.Services;

namespace WebLab3.Controllers
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

        public async Task<IEnumerable<ProductData>> GetAllProductsAsync(string sort, string dir)
        {
            var products = _productRepository.GetAll();

            // Сортировка на основе переданных параметров
            switch (sort.ToLower())
            {
                case "name":
                    products = (dir.ToLower() == "asc")
                        ? products.OrderBy(p => p.Name)
                        : products.OrderByDescending(p => p.Name);
                    break;
                case "executor":
                    products = (dir.ToLower() == "asc")
                        ? products.OrderBy(p => p.Manufacturer)
                        : products.OrderByDescending(p => p.Manufacturer);
                    break;
                case "genre":
                    products = (dir.ToLower() == "asc")
                        ? products.OrderBy(p => p.Barcode)
                        : products.OrderByDescending(p => p.Barcode);
                    break;
                case "purchaseprice":
                    products = (dir.ToLower() == "asc")
                        ? products.OrderBy(p => p.PurchasePrice)
                        : products.OrderByDescending(p => p.PurchasePrice);
                    break;
                case "count":
                    products = (dir.ToLower() == "asc")
                        ? products.OrderBy(p => p.Count)
                        : products.OrderByDescending(p => p.Count);
                    break;
                case "totalprice":
                    products = (dir.ToLower() == "asc")
                        ? products.OrderBy(p => p.PurchasePrice * p.Count)
                        : products.OrderByDescending(p => p.PurchasePrice * p.Count);
                    break;
                default:
                    _logger.LogWarning("Неизвестное поле сортировки {Sort}. Используем сортировку по умолчанию (name)", sort);
                    products = products.OrderBy(p => p.Name);
                    break;
            }
            return await Task.FromResult(products);
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

        public async Task<IEnumerable<ProductData>> SearchProductsAsync(string name, string manufacturer, string barcode, decimal? purchasePrice, int? count)
        {
            // Преобразуем purchasePrice и count в строку или 0, если они не заданы
            var result = _productRepository.Search(name, manufacturer, barcode, purchasePrice?.ToString(), null, count ?? 0);
            return await Task.FromResult(result);
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
    }
}
