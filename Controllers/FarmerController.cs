using AgriEnergyConnect.DTOs;
using AgriEnergyConnect.Models.ViewModels;
using AgriEnergyConnect.Models;
using AgriEnergyConnect.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace AgriEnergyConnect.Controllers
{
    // The FarmerController class handles actions related to farmers in the system.
    // It provides methods for managing products, viewing farmer profiles, and accessing the farmer dashboard.
    // This controller is restricted to users with the "Farmer" role.
    [Authorize(Roles = "Farmer")]
    public class FarmerController : Controller
    {
        // Services for managing farmers and products.
        private readonly IFarmerService _farmerService;

        private readonly IProductService _productService;

        // Constructor for the FarmerController class.
        // Parameters:
        //   farmerService - The service for managing farmers.
        //   productService - The service for managing products.
        public FarmerController(IFarmerService farmerService, IProductService productService)
        {
            _farmerService = farmerService;
            _productService = productService;
        }

        // Displays the farmer dashboard with summary data and recent products.
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var farmerDTO = await _farmerService.GetFarmerDTOByUserIdAsync(userId);

                if (farmerDTO == null)
                    return NotFound();

                ViewBag.Farmer = await _farmerService.GetFarmerByUserIdAsync(userId);

                // Get latest products for dashboard
                var products = await _productService.GetProductsByFarmerIdAsync(farmerDTO.FarmerId);

                // Calculate statistics for dashboard
                ViewBag.TotalProducts = products.Count();
                ViewBag.Categories = products.Select(p => p.Category).Distinct().Count();

                return View(products.OrderByDescending(p => p.CreatedDate).Take(5).ToList());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading dashboard: {ex.Message}";
                return View(Array.Empty<Product>());
            }
        }

        // Displays all products associated with the logged-in farmer.
        [HttpGet]
        public async Task<IActionResult> Products()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var farmer = await _farmerService.GetFarmerByUserIdAsync(userId);

                if (farmer == null)
                    return NotFound();

                ViewBag.Farmer = farmer;

                var products = await _productService.GetProductsByFarmerIdAsync(farmer.FarmerId);
                return View(products);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading products: {ex.Message}";
                return View(Array.Empty<Product>());
            }
        }

        // Filters the farmer's products based on criteria such as date range and category.
        // Parameters:
        //   startDate - The start date of the production date range (optional).
        //   endDate - The end date of the production date range (optional).
        //   category - The category of the products to filter by (optional).
        [HttpGet]
        public async Task<IActionResult> FilterProducts(DateTime? startDate, DateTime? endDate, string category)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var farmer = await _farmerService.GetFarmerByUserIdAsync(userId);

                if (farmer == null)
                    return NotFound();

                var products = await _productService.GetProductsByFilterAsync(farmer.FarmerId, startDate, endDate, category);
                return PartialView("_ProductsTable", products);
            }
            catch (Exception ex)
            {
                // For AJAX calls, return an error message that can be displayed by JavaScript
                return Json(new { success = false, message = $"Error filtering products: {ex.Message}" });
            }
        }

        // Displays the form for adding a new product.
        [HttpGet]
        public IActionResult AddProduct()
        {
            var model = new ProductViewModel { ProductionDate = DateTime.Today };
            return View(model);
        }

        // Handles the submission of the form for adding a new product.
        // Parameters:
        //   model - The ProductViewModel containing the product's details.
        // Returns a redirect to the products page on success or redisplays the form on failure.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(ProductViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var farmer = await _farmerService.GetFarmerByUserIdAsync(userId);

                if (farmer == null)
                    return NotFound();

                var product = new Product
                {
                    Name = model.Name,
                    Category = model.Category,
                    ProductionDate = model.ProductionDate,
                    Description = model.Description,
                    FarmerId = farmer.FarmerId,
                    CreatedDate = DateTime.Now
                };

                await _productService.AddProductAsync(product);

                TempData["SuccessMessage"] = $"Product '{model.Name}' was added successfully.";
                return RedirectToAction(nameof(Products));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error adding product: {ex.Message}");
                return View(model);
            }
        }

        // Displays the form for editing an existing product.
        // Parameters:
        //   id - The ID of the product to edit.
        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var farmer = await _farmerService.GetFarmerByUserIdAsync(userId);

                if (farmer == null)
                    return NotFound();

                var product = await _productService.GetProductByIdAsync(id);

                if (product == null || product.FarmerId != farmer.FarmerId)
                    return NotFound();

                var viewModel = new ProductViewModel
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Category = product.Category,
                    ProductionDate = product.ProductionDate,
                    Description = product.Description,
                    FarmerId = product.FarmerId
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading product: {ex.Message}";
                return RedirectToAction(nameof(Products));
            }
        }

        // Handles the submission of the form for editing an existing product.
        // Parameters:
        //   model - The ProductViewModel containing the updated product details.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(ProductViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var farmer = await _farmerService.GetFarmerByUserIdAsync(userId);

                if (farmer == null)
                    return NotFound();

                var product = await _productService.GetProductByIdAsync(model.ProductId);

                if (product == null || product.FarmerId != farmer.FarmerId)
                    return NotFound();

                product.Name = model.Name;
                product.Category = model.Category;
                product.ProductionDate = model.ProductionDate;
                product.Description = model.Description;

                await _productService.UpdateProductAsync(product);

                TempData["SuccessMessage"] = $"Product '{model.Name}' was updated successfully.";
                return RedirectToAction(nameof(Products));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error updating product: {ex.Message}");
                return View(model);
            }
        }

        // Displays detailed information about a specific product.
        // Parameters:
        //   id - The ID of the product to view.
        [HttpGet]
        public async Task<IActionResult> ProductDetails(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var farmer = await _farmerService.GetFarmerByUserIdAsync(userId);

                if (farmer == null)
                    return NotFound();

                var productDTO = await _productService.GetProductDTOByIdAsync(id);

                if (productDTO == null || productDTO.FarmerId != farmer.FarmerId)
                    return NotFound();

                return View(productDTO);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading product details: {ex.Message}";
                return RedirectToAction(nameof(Products));
            }
        }

        // Displays the profile of the logged-in farmer.
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var farmerDTO = await _farmerService.GetFarmerDTOByUserIdAsync(userId);

                if (farmerDTO == null)
                    return NotFound();

                return View(farmerDTO);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading profile: {ex.Message}";
                return RedirectToAction(nameof(Dashboard));
            }
        }

        // Searches for products based on a search term.
        // Parameters:
        //   term - The term to search for in product names or descriptions.
        [HttpGet]
        public async Task<IActionResult> SearchProducts(string term)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var farmer = await _farmerService.GetFarmerByUserIdAsync(userId);

                if (farmer == null)
                    return NotFound();

                var filter = new ProductFilterDTO
                {
                    FarmerId = farmer.FarmerId,
                    SearchTerm = term
                };

                var products = await _productService.GetFilteredProductDTOsAsync(filter);
                return Json(products);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}