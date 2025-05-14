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
    public class HomeController : Controller
    {
        private readonly IProductService _productService;

        public HomeController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index(string sort = "name", string dir = "asc")
        {
            var products = await _productService.GetAllProductsAsync(sort, dir);
            ViewBag.CurrentSort = sort;
            ViewBag.CurrentDir = dir.ToLower();
            return View(products);
        }

        public async Task<IActionResult> Search(string name, string manufacturer, string barcode, string purchasePrice, string totalPrice, int count)
        {
            var products = await _productService.SearchProductsAsync(name, manufacturer, barcode, purchasePrice, totalPrice, count);

            return View("Index", products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        public IActionResult CreateProduct()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductCreationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            await _productService.CreateProductAsync(viewModel);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> EditProduct(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductEditViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Manufacturer = product.Manufacturer,
                Barcode = product.Barcode,
                PurchasePrice = product.PurchasePrice,
                Count = product.Count
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            await _productService.UpdateProductAsync(viewModel);
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> DeleteProduct(int productId)
        {
            await _productService.DeleteProductAsync(productId);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Create50Product()
        {
            await _productService.Create50RandomProductsAsync();
            return RedirectToAction(nameof(Index));
        }
    }

}
