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
        private readonly IFarmerDashboardService _dashboardService;

        // Constructor for the FarmerController class.
        // Parameters:
        //   farmerService - The service for managing farmers.
        //   productService - The service for managing products.
        //   dashboardService - The service for farmer dashboard operations.
        public FarmerController(
            IFarmerService farmerService,
            IProductService productService,
            IFarmerDashboardService dashboardService)
        {
            _farmerService = farmerService;
            _productService = productService;
            _dashboardService = dashboardService;
        }

        // Displays the farmer dashboard with summary data and recent products.
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Get dashboard data using the dashboard service
                var (farmer, totalProducts, categoryCount, lastAddedTime, categoryList,
                     recentProducts, recentActivities, lastProductAdded) =
                    await _dashboardService.GetDashboardDataAsync(userId);

                // Set ViewBag properties for the view
                ViewBag.Farmer = farmer;
                ViewBag.TotalProducts = totalProducts;
                ViewBag.Categories = categoryCount;
                ViewBag.CategoryList = categoryList;
                ViewBag.LastAdded = lastAddedTime;
                ViewBag.RecentProducts = recentProducts;
                ViewBag.RecentActivities = recentActivities;
                ViewBag.LastProductAdded = lastProductAdded ?? DateTime.Now;
                ViewBag.LastAddedTimeAgo = lastAddedTime;

                // Return the view with recent products as the model (for backward compatibility)
                return View(recentProducts);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading dashboard: {ex.Message}";
                return View(Array.Empty<Product>());
            }
        }

        // Displays all products associated with the logged-in farmer.
        [HttpGet]
        public async Task<IActionResult> Products(
            string searchTerm, string selectedCategory, DateTime? filterStartDate, DateTime? filterEndDate)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var farmer = await _farmerService.GetFarmerByUserIdAsync(userId);

                if (farmer == null)
                    return NotFound();

                // Get filtered products and create view model
                var viewModel = await _productService.GetFarmerProductsAsync(
                    farmer.FarmerId, searchTerm, selectedCategory, filterStartDate, filterEndDate);

                return View("MyProducts", viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading products: {ex.Message}";
                return View("MyProducts", new FarmerProductsViewModel());
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

                // Get filtered products
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

                // Create product using service method
                await _productService.CreateProductFromViewModelAsync(model, farmer.FarmerId);

                TempData["ProductSuccessMessage"] = $"Product '{model.Name}' was added successfully.";
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

                // Get product for editing using service method
                var viewModel = await _productService.GetProductForEditingAsync(id, farmer.FarmerId);
                return View(viewModel);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
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

                // Update product using service method
                await _productService.UpdateProductFromViewModelAsync(model, farmer.FarmerId);

                TempData["ProductSuccessMessage"] = $"Product '{model.Name}' was updated successfully.";
                return RedirectToAction(nameof(Products));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
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

        [HttpGet]
        public async Task<IActionResult> GetEditProductModal(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var farmer = await _farmerService.GetFarmerByUserIdAsync(userId);

                if (farmer == null)
                    return Unauthorized();

                // Get product for editing
                var viewModel = await _productService.GetProductForEditingAsync(id, farmer.FarmerId);
                return PartialView("_EditProductModal", viewModel);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Could not load product: {ex.Message}" });
            }
        }
    }
}