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

                // Get farmer data using existing DTO method
                var farmerDTO = await _farmerService.GetFarmerDTOByUserIdAsync(userId);
                if (farmerDTO == null)
                    return NotFound();

                // Get all product DTOs for the farmer using existing method
                var productDTOs = await _productService.GetProductDTOsByFarmerIdAsync(farmerDTO.FarmerId);
                var productList = productDTOs.ToList();

                // Calculate statistics
                var totalProducts = productList.Count;
                var categoryCount = productList.Select(p => p.Category).Distinct().Count();
                var categoryList = productList.Select(p => p.Category).Distinct().ToList();

                // Get the last added product time
                var lastProduct = productList.OrderByDescending(p => p.CreatedDate).FirstOrDefault();
                string lastAddedTime;
                if (lastProduct != null)
                {
                    var timeSpan = DateTime.Now - lastProduct.CreatedDate;
                    lastAddedTime = FormatTimeAgo(timeSpan);
                }
                else
                {
                    lastAddedTime = "No products yet";
                }

                // Create farmer object from DTO for view compatibility
                var farmer = new Farmer
                {
                    FarmerId = farmerDTO.FarmerId,
                    FarmName = farmerDTO.FarmName,
                    Location = farmerDTO.Location,
                    User = new User
                    {
                        FirstName = farmerDTO.FirstName,
                        LastName = farmerDTO.LastName,
                        CreatedDate = farmerDTO.JoinedDate,
                        Role = UserRole.Farmer
                    }
                };

                // Create recent products list
                var recentProducts = ConvertProductDTOsToProducts(productList.OrderByDescending(p => p.CreatedDate).Take(5).ToList());

                // Create recent activities
                var recentActivities = GetRecentActivitiesFromDTOs(productList);

                // Set ViewBag properties for the view (cast to dynamic to avoid casting issues)
                ViewBag.Farmer = farmer;
                ViewBag.TotalProducts = totalProducts;
                ViewBag.Categories = categoryCount;
                ViewBag.CategoryList = categoryList;
                ViewBag.LastAdded = lastAddedTime;
                ViewBag.RecentProducts = recentProducts;
                ViewBag.RecentActivities = recentActivities;
                ViewBag.LastProductAdded = lastProduct?.CreatedDate ?? DateTime.Now;
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

        // Helper method to convert ProductDTOs to Product entities for view compatibility
        private List<Product> ConvertProductDTOsToProducts(List<ProductDTO> productDTOs)
        {
            return productDTOs.Select(dto => new Product
            {
                ProductId = dto.ProductId,
                Name = dto.Name,
                Category = dto.Category,
                Description = dto.Description,
                ProductionDate = dto.ProductionDate,
                FarmerId = dto.FarmerId,
                CreatedDate = dto.CreatedDate
            }).ToList();
        }

        // Helper method to format time ago
        private string FormatTimeAgo(TimeSpan timeSpan)
        {
            if (timeSpan.TotalDays >= 365)
            {
                int years = (int)(timeSpan.TotalDays / 365);
                return $"{years} year{(years > 1 ? "s" : "")} ago";
            }
            if (timeSpan.TotalDays >= 30)
            {
                int months = (int)(timeSpan.TotalDays / 30);
                return $"{months} month{(months > 1 ? "s" : "")} ago";
            }
            if (timeSpan.TotalDays >= 1)
            {
                int days = (int)timeSpan.TotalDays;
                return $"{days} day{(days > 1 ? "s" : "")} ago";
            }
            if (timeSpan.TotalHours >= 1)
            {
                int hours = (int)timeSpan.TotalHours;
                return $"{hours} hour{(hours > 1 ? "s" : "")} ago";
            }
            if (timeSpan.TotalMinutes >= 1)
            {
                int minutes = (int)timeSpan.TotalMinutes;
                return $"{minutes} minute{(minutes > 1 ? "s" : "")} ago";
            }
            return "just now";
        }

        // Helper method to generate recent activities from actual ProductDTOs
        private List<ActivityItem> GetRecentActivitiesFromDTOs(List<ProductDTO> productDTOs)
        {
            var activities = new List<ActivityItem>();

            // Get all products ordered by creation date to show actual activity
            var sortedProducts = productDTOs.OrderByDescending(p => p.CreatedDate).ToList();

            // Add activity items for each product creation
            foreach (var product in sortedProducts)
            {
                activities.Add(new ActivityItem
                {
                    Description = $"Added \"{product.Name}\"",
                    Time = FormatTimeAgo(DateTime.Now - product.CreatedDate),
                    Color = "#4CAF50",
                    Icon = "fa-plus"
                });
            }

            return activities;
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