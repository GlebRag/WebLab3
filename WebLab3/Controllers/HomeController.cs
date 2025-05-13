using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using WebLab3.Data;
using WebLab3.Data.Models;
using WebLab3.Data.Repositories;
using WebLab3.Models;
using WebLab3.Models.Product;


namespace WebLab3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IProductRepositoryReal _productRepository;
        private WebDbContext _webDbContext;
        public HomeController(ILogger<HomeController> logger,
            IProductRepositoryReal productRepository,
            WebDbContext webDbContext)
        {
            _productRepository = productRepository;
            _logger = logger;
            _webDbContext = webDbContext;
        }

        public IActionResult Index(string sort, string dir)
        {
            // Если не задан столбец сортировки, по умолчанию сортируем по названию
            if (string.IsNullOrEmpty(sort))
            {
                sort = "name";
            }
            // Если направление сортировки не задано, по умолчанию ASC
            if (string.IsNullOrEmpty(dir))
            {
                dir = "asc";
            }

            IEnumerable<ProductData> productData = _productRepository.GetAll();

            switch (sort.ToLower())
            {
                case "name":
                    productData = (dir.ToLower() == "asc") ? productData.OrderBy(v => v.Name) : productData.OrderByDescending(v => v.Name);
                    break;
                case "executor":
                    productData = (dir.ToLower() == "asc") ? productData.OrderBy(v => v.Manufacturer) : productData.OrderByDescending(v => v.Manufacturer);
                    break;
                case "genre":
                    productData = (dir.ToLower() == "asc") ? productData.OrderBy(v => v.Barcode) : productData.OrderByDescending(v => v.Barcode);
                    break;
                case "purchaseprice":
                    productData = (dir.ToLower() == "asc") ? productData.OrderBy(v => v.PurchasePrice) : productData.OrderByDescending(v => v.PurchasePrice);
                    break;
                case "count":
                    productData = (dir.ToLower() == "asc") ? productData.OrderBy(v => v.Count) : productData.OrderByDescending(v => v.Count);
                    break;
                case "totalprice":
                    productData = (dir.ToLower() == "asc") ? productData.OrderBy(v => v.PurchasePrice * v.Count) : productData.OrderByDescending(v => v.PurchasePrice * v.Count);
                    break;
                default:
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

            ViewBag.CurrentSort = sort;
            ViewBag.CurrentDir = dir.ToLower();


            return View("Index", model);
        }



        [HttpGet]
        public IActionResult CreateProduct()
        {

            var viewModel = new ProductCreationViewModel();

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult CreateProduct(ProductCreationViewModel viewModel)
        {

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var dataProduct = new ProductData
            {
                Name = viewModel.Name,
                Manufacturer = viewModel.Manufacturer,
                Barcode = viewModel.Barcode,
                PurchasePrice = viewModel.PurchasePrice,
                Count = viewModel.Count,
            };

            _productRepository.Create(dataProduct);


            return RedirectToAction("Index");
        }

        public IActionResult DeleteProduct(int productId)
        {

            _productRepository.Delete(productId);


            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult EditProduct(int productId)
        {

            var viewModel = new ProductEditViewModel();

            var product = _webDbContext.Products.First(x => x.Id == productId);
            viewModel.Name = product.Name;
            viewModel.Manufacturer = product.Manufacturer;
            viewModel.Barcode = product.Barcode;
            viewModel.PurchasePrice = product.PurchasePrice;
            viewModel.Count = product.Count;
            viewModel.Id = productId;

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult EditProduct(ProductEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {

                return View(viewModel);
            }

            var productId = viewModel.Id;


            var dataProduct = new ProductData
            {
                Id = viewModel.Id,
                Name = viewModel.Name,
                Manufacturer = viewModel.Manufacturer,
                Barcode = viewModel.Barcode,
                PurchasePrice = viewModel.PurchasePrice,
                Count = viewModel.Count
                
            };


            _productRepository.Update(dataProduct, productId);



            return RedirectToAction("Index");
        }
        public IActionResult Search(string name, string manufacturer, string barcode, string purchasePrice, string totalPrice, int count)
        {

            var viewModels = _productRepository
                .Search(name, manufacturer, barcode,  purchasePrice,  totalPrice,  count)
                .Select(dbBook => new ProductViewModel
                {

                    Id = dbBook.Id,
                    Name = dbBook.Name,
                    Manufacturer = dbBook.Manufacturer,
                    Barcode = dbBook.Barcode,
                    PurchasePrice = dbBook.PurchasePrice,
                    Count = dbBook.Count,
                    TotalPrice = dbBook.PurchasePrice * dbBook.Count

                })
                .ToList();

            return View("Index", viewModels);
        }
        public IActionResult Create50Product()
        {
            var random = new Random();

            // Список случайных товаров
            string[] productNames = new[]
            {
        "Телевизор", "Холодильник", "Смартфон", "Ноутбук", "Кофемашина",
        "Часы", "Кондиционер", "Велосипед", "Фотоаппарат", "Игровая приставка"
    };

            // Список стран-производителей
            string[] manufacturers = new[]
            {
        "США", "Германия", "Япония", "Китай", "Южная Корея",
        "Франция", "Италия", "Россия", "Канада", "Великобритания"
    };

            // Диапазоны для цены и количества
            decimal priceMin = 5.00M;
            decimal priceMax = 50.00M;

            for (int i = 0; i < 50; i++)
            {
                // Выбор случайных значений из массивов
                string randomName = productNames[random.Next(productNames.Length)];
                string randomManufacturer = manufacturers[random.Next(manufacturers.Length)];

                // Генерация случайного 8-значного числа для баркода (в виде строки)
                string randomBarcode = random.Next(10000000, 99999999).ToString();

                // Генерация случайной цены типа decimal
                decimal randomPrice = Math.Round((decimal)(random.NextDouble() * ((double)priceMax - (double)priceMin) + (double)priceMin), 2);

                // Генерация количества в диапазоне от 1 до 20
                int randomCount = random.Next(1, 21);

                var dataModel = new ProductData
                {
                    Name = randomName,
                    Manufacturer = randomManufacturer,
                    Barcode = randomBarcode,
                    PurchasePrice = randomPrice,
                    Count = randomCount
                };

                _productRepository.Add(dataModel);
            }

            return RedirectToAction("Index");
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
