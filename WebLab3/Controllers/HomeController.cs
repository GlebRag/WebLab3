using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.IO;
using WebLab3.Data;
using WebLab3.Data.Models;
using WebLab3.Data.Repositories;
using WebLab3.Models;
using WebLab3.Models.Product;



namespace WebLab3.Controllers
{
    /// <summary>
    /// �������������� ������������:
    /// 1 God Object ����� UniversalManager ���������� ����: ������������, ��������������, ��������� ������������, ���������� ����� ������ � ���� ������������ ��������. �� ����� �� ��� ���.

    // 2 Singleton Overuse UniversalManager ���������� ��� �������� � ������������ �� ���� ������� �����������, ��� ������ ���������� ��������� � ���������� ������������.

    // 3 Magic Numbers ����� 137 ������������ � �������� �������� � ���������� ������ ��� ����������, ������ ������ ������ 137.


    // 4 Hardcoded File Paths ���������� ���� ������ � ����, ��� ������� ������������� � ��������.

    // 5 Boat Anchor ����� DeprecatedMethod ������� � ����, ���� ��� ��� �� ���������� � �� ������ �������� ����� � ������ �����������.

    // 6 Copy-Paste Programming ��������� �������� ����(��������, ������� ������� � ��������� Index � Search) �����������, ������� ������� DRY(Don't Repeat Yourself).


    // 7 Lava Flow ���������� ������ � �������� ������������, ������� ���������� � �� ����� ������ ������, �������� ������� ����.

    // 8 Inner Platform Effect ����������� ���� ����������� ������ ���������� � �������� ������������, ����������� ���������� ASP.NET � ����������� ���������.


    // 9 Sequential Coupling ��������� ������ (��������, �������� � ���������� ��������) �������������, ��� ���������� ��� ��� ��������������� ����������� ����������(����� _manager.Initialize()), ����� ���������� ����� �� �������� ���������.

    // 10 Excessive Logging ����������� ������������ �� ������ ����, ������ ���� �������� (��� � ������� Index � Create50Product), ��� �������� � ���������� ���������� � ��������� ������ �����.

    /// </summary>
    
    // ������������� �������� � God Object, Singleton Overuse, Inner Platform Effect, � ����� ���������� Magic Numbers, Hardcoded File Paths � Excessive Logging
    public sealed class UniversalManager
    {
        private static readonly UniversalManager _instance = new UniversalManager();
        public static UniversalManager Instance => _instance;

        // Magic number, ������������ ��� �������� �� ���� ������� � ����� �� �����, ������ ������ 137!
        private int _magicDelay = 137;

        // Hardcoded file path ��� ����������� � �� ���������� ����� ���������!
        private string _logFilePath = "D:\\appLog.txt";

        private bool _initialized = false;

        // Excessive Logging � �������� ������ ��������
        public void Log(string message)
        {
            string logEntry = DateTime.Now.ToString("o") + " - " + message;
            Console.WriteLine(logEntry);
            try
            {
                File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
            }
            catch (Exception)
            {
                // swallow exception
            }
        }

        // Sequential Coupling � ������ ���� ������ ��� ������� ����������
        public void Initialize()
        {
            if (!_initialized)
            {
                Log("UniversalManager initialization started.");
                Thread.Sleep(_magicDelay);
                _initialized = true;
                Log("UniversalManager initialization finished.");
            }
        }

        // Hardcoded File Paths ��� ����������������� �����
        private void LoadConfiguration()
        {
            string configPath = "C:\\Project\\Config\\appsettings.json";
            Log("Loading configuration from " + configPath);
            // �������� �������� ������������ � Boat Anchor: ���, ������� ������� �� ��������
            if (!File.Exists(configPath))
            {
                File.WriteAllText(configPath, "{\"default\":\"value\"}");
                Log("Default configuration created.");
            }
        }

        // Boat Anchor � �����, ������� �������� ���� ���� �� ������ �� �����
        public void DumpDataToFile(string data)
        {
            try
            {
                File.AppendAllText(_logFilePath, "Dump: " + data + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Log("Failed to dump data: " + ex.Message);
            }
        }

        // Copy-Paste Programming & Lava Flow � ���������� �����-�����, ������ �� ��������
        public void DumpDataToFile(string data, bool duplicate)
        {
            DumpDataToFile(data);
            if (duplicate)
            {
                DumpDataToFile(data); // ����� ���� ���, ����� ����� ����������� ���?
            }
        }
    }

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IProductRepositoryReal _productRepository;
        private WebDbContext _webDbContext;

        // ���������� ��� ���������� UniversalManager � ���������� ���������, ��������� �����
        private readonly UniversalManager _manager = UniversalManager.Instance;

        public HomeController(ILogger<HomeController> logger,
            IProductRepositoryReal productRepository,
            WebDbContext webDbContext)
        {
            _logger = logger;
            _productRepository = productRepository;
            _webDbContext = webDbContext;

            // Sequential Coupling � ������� ������������� ��������� � ������������
            _manager.Initialize();
        }

        public IActionResult Index(string sort, string dir)
        {
            _manager.Log($"Index action started with sort='{sort}' and dir='{dir}'");
            Thread.Sleep(137); // Magic Number

            // ���� ���������� �� ������, ������������ "name" � ������ ������� default
            if (string.IsNullOrEmpty(sort))
                sort = "name";
            if (string.IsNullOrEmpty(dir))
                dir = "asc";

            IEnumerable<ProductData> productData = _productRepository.GetAll();

            // Inner Platform Effect: ����������� ���������� ����������, ����������� ����������� ����������
            switch (sort.ToLower())
            {
                case "name":
                    productData = (dir.ToLower() == "asc")
                        ? productData.OrderBy(v => v.Name)
                        : productData.OrderByDescending(v => v.Name);
                    break;
                case "executor":
                    productData = (dir.ToLower() == "asc")
                        ? productData.OrderBy(v => v.Manufacturer)
                        : productData.OrderByDescending(v => v.Manufacturer);
                    break;
                case "genre":
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
                    _manager.Log("Unknown sort field, defaulting to name");
                    productData = productData.OrderBy(v => v.Name);
                    break;
            }

            // Copy-Paste Programming � ������������� �������������� ������
            var model = productData.Select(v =>
            {
                _manager.Log("Mapping product: " + v.Name);
                return new ProductViewModel
                {
                    Id = v.Id,
                    Name = v.Name,
                    Manufacturer = v.Manufacturer,
                    Barcode = v.Barcode,
                    PurchasePrice = v.PurchasePrice,
                    Count = v.Count,
                    TotalPrice = v.PurchasePrice * v.Count
                };
            }).ToList();

            // Hardcoded ������ ����� ViewBag � ���������� ��������
            ViewBag.CurrentSort = sort;
            ViewBag.CurrentDir = dir.ToLower();

            _manager.DumpDataToFile("Index action processed " + model.Count + " products");
            _manager.Log("Index action completed.");

            return View("Index", model);
        }

        [HttpGet]
        public IActionResult CreateProduct()
        {
            _manager.Log("CreateProduct GET action started.");
            // Copy-Paste Programming � ������� �������� ������ ��� �������������� ������
            var viewModel = new ProductCreationViewModel();
            _manager.Log("Returning view for CreateProduct.");
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult CreateProduct(ProductCreationViewModel viewModel)
        {
            _manager.Log("CreateProduct POST action started.");
            if (!ModelState.IsValid)
            {
                _manager.Log("Invalid ModelState in CreateProduct POST.");
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

            // Sequential Coupling � ������������, ��� �������� ���������� ��� ��������������� ��������
            _productRepository.Create(dataProduct);
            _manager.Log("Product created: " + viewModel.Name);
            return RedirectToAction("Index");
        }

        public IActionResult DeleteProduct(int productId)
        {
            _manager.Log("DeleteProduct action started for productId: " + productId);
            Thread.Sleep(137); // Magic Number
            _productRepository.Delete(productId);
            _manager.Log("Product deleted with id: " + productId);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult EditProduct(int productId)
        {
            _manager.Log("EditProduct GET action started for productId: " + productId);
            var viewModel = new ProductEditViewModel();

            // Copy-Paste Programming � ���������������� ���������� ������ ��� ������������� ��������������� �������
            var product = _webDbContext.Products.First(x => x.Id == productId);
            viewModel.Name = product.Name;
            viewModel.Manufacturer = product.Manufacturer;
            viewModel.Barcode = product.Barcode;
            viewModel.PurchasePrice = product.PurchasePrice;
            viewModel.Count = product.Count;
            viewModel.Id = productId;

            _manager.Log("EditProduct GET ready with data for product: " + product.Name);
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult EditProduct(ProductEditViewModel viewModel)
        {
            _manager.Log("EditProduct POST action started for productId: " + viewModel.Id);
            if (!ModelState.IsValid)
            {
                _manager.Log("Invalid ModelState in EditProduct POST.");
                return View(viewModel);
            }

            var dataProduct = new ProductData
            {
                Id = viewModel.Id,
                Name = viewModel.Name,
                Manufacturer = viewModel.Manufacturer,
                Barcode = viewModel.Barcode,
                PurchasePrice = viewModel.PurchasePrice,
                Count = viewModel.Count
            };

            // Sequential Coupling � ���������� �������� ��� �������� �������� ������
            _productRepository.Update(dataProduct, viewModel.Id);
            _manager.Log("Product updated with id: " + viewModel.Id);
            return RedirectToAction("Index");
        }

        public IActionResult Search(string name, string manufacturer, string barcode, string purchasePrice, string totalPrice, int count)
        {
            _manager.Log("Search action started with parameters: " +
                $"name={name}, manufacturer={manufacturer}, barcode={barcode}, purchasePrice={purchasePrice}, totalPrice={totalPrice}, count={count}");

            // Copy-Paste Programming � �������������� ��������� ������ ��������� ������ �� Index
            var viewModels = _productRepository
                .Search(name, manufacturer, barcode, purchasePrice, totalPrice, count)
                .Select(dbBook =>
                {
                    _manager.Log("Mapping search result for product: " + dbBook.Name);
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

            _manager.Log("Search action completed with result count: " + viewModels.Count);
            return View("Index", viewModels);
        }

        public IActionResult Create50Product()
        {
            _manager.Log("Create50Product action started.");
            var random = new Random();

            // ������ ��������� �������
            string[] productNames = new[]
            {
                "���������", "�����������", "��������", "�������", "����������",
                "����", "�����������", "���������", "�����������", "������� ���������"
            };

            // ������ �����-��������������
            string[] manufacturers = new[]
            {
                "���", "��������", "������", "�����", "����� �����",
                "�������", "������", "������", "������", "��������������"
            };

            // ��������� ��� ���� � ����������
            decimal priceMin = 5.00M;
            decimal priceMax = 50.00M;

            for (int i = 0; i < 50; i++)
            {
                _manager.Log("Creating product " + (i + 1) + " of 50");
                string randomName = productNames[random.Next(productNames.Length)];
                string randomManufacturer = manufacturers[random.Next(manufacturers.Length)];
                string randomBarcode = random.Next(10000000, 99999999).ToString();
                decimal randomPrice = Math.Round((decimal)(random.NextDouble() * ((double)priceMax - (double)priceMin) + (double)priceMin), 2);
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
                // Excessive Logging ���� �������� ��� ������� �������� � Magic Number
                Thread.Sleep(137);
            }

            _manager.DumpDataToFile("Created 50 random products at " + DateTime.Now.ToString());
            _manager.Log("Create50Product action completed.");
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            _manager.Log("Privacy action called.");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            _manager.Log("Error action triggered.");
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // 5. Boat Anchor & 7. Lava Flow � ���������� �����, ������� ������� �� ����������,
        // �� �������� � ������� ���� � ���� ������ � �������.
        public void DeprecatedMethod()
        {
            _manager.Log("DeprecatedMethod has been called.");
            for (int i = 0; i < 10; i++)
            {
                _manager.Log("DeprecatedMethod iteration " + i);
            }
        }
    }

}
