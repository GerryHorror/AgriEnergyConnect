using AgriEnergyConnect.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AgriEnergyConnect.Models.ViewModels;
using AgriEnergyConnect.Models;
using AgriEnergyConnect.DTOs;
using AgriEnergyConnect.Services.Implementation;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace AgriEnergyConnect.Controllers
{
    // The EmployeeController class handles actions related to employees in the system.
    // It provides methods for managing farmers, products, and dashboard data.
    // This controller is restricted to users with the "Employee" role.
    [Authorize(Roles = "Employee")]
    public class EmployeeController : Controller
    {
        // Services for managing farmers and products.
        private readonly IFarmerService _farmerService;

        private readonly IProductService _productService;

        // The AuthService is used for user authentication and retrieval.
        private readonly IAuthService _authService;

        // The DashboardService is used for retrieving dashboard data.
        private readonly IDashboardService _dashboardService;

        // Constructor for the EmployeeController class.
        // Parameters:
        //   farmerService - The service for managing farmers.
        //   productService - The service for managing products.
        //   authService - The service for user authentication.
        //   dashboardService - The service for retrieving dashboard data.
        public EmployeeController(IFarmerService farmerService, IProductService productService, IAuthService authService, IDashboardService dashboardService)
        {
            _farmerService = farmerService;
            _productService = productService;
            _authService = authService;
            _dashboardService = dashboardService;
        }

        // Displays the employee dashboard with summary data.
        // Retrieves total farmers, total products, recent farmers, and active users.
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var viewModel = await _dashboardService.GetDashboardDataAsync(userId);

                // Set required ViewBag properties from the viewModel
                ViewBag.Employee = new { User = viewModel.Employee };
                ViewBag.TotalFarmers = viewModel.TotalFarmers;
                ViewBag.TotalProducts = viewModel.TotalProducts;
                ViewBag.ActiveUsers = viewModel.ActiveUsers;
                ViewBag.NewFarmers = viewModel.NewFarmers;
                ViewBag.NewProducts = viewModel.NewProducts;
                ViewBag.ActivePercentage = viewModel.ActivePercentage;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading dashboard: {ex.Message}";
                return View(new EmployeeDashboardViewModel());
            }
        }

        // Helper method to parse the time string for sorting
        private DateTime ParseTime(string timeString)
        {
            if (timeString == "Just now")
                return DateTime.Now;
            if (timeString.Contains("minutes ago"))
                return DateTime.Now.AddMinutes(-int.Parse(timeString.Split(' ')[0]));
            if (timeString.Contains("Today"))
            {
                var timePart = timeString.Split(',')[1].Trim();
                return DateTime.Parse($"{DateTime.Now.ToShortDateString()} {timePart}");
            }
            if (timeString.Contains("Yesterday"))
            {
                var timePart = timeString.Split(',')[1].Trim();
                return DateTime.Parse($"{DateTime.Now.AddDays(-1).ToShortDateString()} {timePart}");
            }
            if (timeString.Contains("days ago"))
            {
                var days = int.Parse(timeString.Split(' ')[0]);
                return DateTime.Now.AddDays(-days);
            }

            // Default to parsing the full date string
            return DateTime.TryParse(timeString, out DateTime result) ? result : DateTime.Now;
        }

        // Displays the form for adding a new farmer.
        [HttpGet]
        public IActionResult AddFarmer()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = _authService.GetUserByIdAsync(userId).Result;

            ViewBag.Employee = new { User = user };

            return View(new FarmerViewModel());
        }

        // Handles the submission of the form for adding a new farmer.
        // Parameters:
        //   model - The FarmerViewModel containing the farmer's details.
        // Returns a redirect to the dashboard on success or redisplays the form on failure.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFarmer(FarmerViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                // Use the service method to create the farmer
                var farmer = await _farmerService.CreateFarmerFromViewModelAsync(model);

                TempData["SuccessMessage"] = $"Farmer {model.FirstName} {model.LastName} was successfully added.";
                return RedirectToAction(nameof(Dashboard));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred while adding the farmer: {ex.Message}");
                return View(model);
            }
        }

        // Displays the products associated with a specific farmer.
        // Parameters:
        //   farmerId - The ID of the farmer whose products are to be displayed.
        [HttpGet]
        public async Task<IActionResult> ViewFarmerProducts(int farmerId, string searchTerm = "", string category = "", string statusFilter = "", DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var user = await _authService.GetUserByIdAsync(userId);

                if (user == null)
                    return NotFound();

                // Set ViewBag employee data
                ViewBag.Employee = new { User = user };

                // Get the farmer to display farmer information
                var farmer = await _farmerService.GetFarmerByIdAsync(farmerId);
                if (farmer == null)
                    return NotFound();

                ViewBag.Farmer = farmer;

                // Get filtered products and categories
                var (products, categories) = await _productService.GetFarmerProductsForViewAsync(
                    farmerId, searchTerm, category, statusFilter, startDate, endDate);

                // Set ViewBag properties for the view
                ViewBag.Categories = categories;
                ViewBag.SearchTerm = searchTerm;
                ViewBag.SelectedCategory = category;
                ViewBag.SelectedStatus = statusFilter;
                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;

                return View(products);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading products: {ex.Message}";
                return View(Enumerable.Empty<ProductDTO>());
            }
        }

        // Filters the products of a specific farmer based on criteria.
        // Parameters:
        //   farmerId - The ID of the farmer whose products are to be filtered.
        //   startDate - The start date of the production date range (optional).
        //   endDate - The end date of the production date range (optional).
        //   category - The category of the products to filter by (optional).
        [HttpGet]
        public async Task<IActionResult> FilterFarmerProducts(int farmerId, DateTime? startDate, DateTime? endDate, string category)
        {
            var farmer = await _farmerService.GetFarmerByIdAsync(farmerId);

            if (farmer == null)
                return NotFound();

            var products = await _productService.GetProductsByFilterAsync(farmerId, startDate, endDate, category);
            return PartialView("_ProductList", products);
        }

        // Displays detailed information about a specific farmer.
        // Parameters:
        //   farmerId - The ID of the farmer whose details are to be displayed.
        [HttpGet]
        public async Task<IActionResult> FarmerDetails(int farmerId)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var user = await _authService.GetUserByIdAsync(userId);

                if (user == null)
                    return NotFound();

                // Set ViewBag employee data
                ViewBag.Employee = new { User = user };

                // Get detailed farmer information
                var farmerDTO = await _farmerService.GetFarmerDetailsForViewAsync(farmerId);

                return View(farmerDTO);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error retrieving farmer details: {ex.Message}";
                return RedirectToAction("ManageFarmers");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Products(string searchTerm = "", string category = "", string farmerFilter = "", string statusFilter = "", DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 10)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var user = await _authService.GetUserByIdAsync(userId);

                if (user == null)
                    return NotFound();

                // Set ViewBag employee data
                ViewBag.Employee = new { User = user };

                // Get all farmers for the dropdown filter
                var farmers = await _farmerService.GetAllFarmerSummariesAsync();

                // Get filtered products
                var (products, categories, totalProducts, totalPages) =
                    await _productService.GetProductsForAdminViewAsync(
                        searchTerm, category, farmerFilter, statusFilter,
                        startDate, endDate, page, pageSize);

                // Pass all necessary data to the view
                ViewBag.Farmers = farmers;
                ViewBag.Categories = categories;
                ViewBag.SearchTerm = searchTerm;
                ViewBag.SelectedCategory = category;
                ViewBag.SelectedFarmer = farmerFilter;
                ViewBag.SelectedStatus = statusFilter;
                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalProducts = totalProducts;

                return View(products);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading products: {ex.Message}";
                return View(new List<ProductDTO>());
            }
        }

        // Displays detailed information about a specific product.
        // Parameters:
        //   productId - The ID of the product whose details are to be displayed.
        [HttpGet]
        public async Task<IActionResult> ProductDetails(int productId)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var user = await _authService.GetUserByIdAsync(userId);

                if (user == null)
                    return NotFound();

                // Set ViewBag employee data
                ViewBag.Employee = new { User = user };

                // Get product details and associated farmer
                var (productDTO, farmer) = await _productService.GetProductDetailsForViewAsync(productId);

                // Set ViewBag.Farmer for navigation breadcrumbs
                ViewBag.Farmer = farmer;

                return View(productDTO);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error retrieving product details: {ex.Message}";
                return RedirectToAction("Products");
            }
        }

        // Searches for products based on a search term, category, or farmer ID.
        // Parameters:
        //   searchTerm - The term to search for in product names or descriptions (optional).
        //   category - The category to filter products by (optional).
        //   farmerId - The ID of the farmer whose products are to be searched (optional).
        [HttpGet]
        public async Task<IActionResult> SearchProducts(string searchTerm = "", string category = "", int? farmerId = null)
        {
            var filter = new ProductFilterDTO
            {
                SearchTerm = searchTerm,
                Category = category,
                FarmerId = farmerId
            };

            var products = await _productService.GetFilteredProductDTOsAsync(filter);
            return PartialView("_ProductList", products);
        }

        // Displays a list of all farmers for management purposes.
        [HttpGet]
        public async Task<IActionResult> ManageFarmers(string searchTerm = "", string locationFilter = "", string statusFilter = "", int page = 1, int pageSize = 10)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var user = await _authService.GetUserByIdAsync(userId);

                if (user == null)
                    return NotFound();

                // Set ViewBag employee data
                ViewBag.Employee = new { User = user };

                // Get filtered and paginated farmers
                var (farmers, totalPages, totalFarmers, uniqueLocations) =
                    await _farmerService.GetFilteredFarmersAsync(searchTerm, locationFilter, statusFilter, page, pageSize);

                // Set ViewBag data for the view
                ViewBag.SearchTerm = searchTerm;
                ViewBag.SelectedLocation = locationFilter;
                ViewBag.SelectedStatus = statusFilter;
                ViewBag.UniqueLocations = uniqueLocations;
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalFarmers = totalFarmers;

                return View(farmers);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading farmers: {ex.Message}";
                return View(new List<FarmerSummaryDTO>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditFarmer(int farmerId)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var user = await _authService.GetUserByIdAsync(userId);

                if (user == null)
                    return NotFound();

                // Set ViewBag employee data
                ViewBag.Employee = new { User = user };

                // Get farmer information for editing
                var viewModel = await _farmerService.GetFarmerForEditingAsync(farmerId);

                ViewBag.FarmerId = farmerId;
                ViewData["FormContext"] = "Edit";
                return View(viewModel);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading farmer data: {ex.Message}";
                return RedirectToAction("ManageFarmers");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFarmer(int farmerId, FarmerViewModel model)
        {
            ViewData["FormContext"] = "Edit";
            if (!ModelState.IsValid)
            {
                ViewBag.FarmerId = farmerId;
                return View(model);
            }

            try
            {
                // Update the farmer using the service method
                await _farmerService.UpdateFarmerFromViewModelAsync(farmerId, model);

                TempData["SuccessMessage"] = $"Farmer {model.FirstName} {model.LastName} was successfully updated.";
                return RedirectToAction(nameof(ManageFarmers));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred while updating the farmer: {ex.Message}");
                ViewBag.FarmerId = farmerId;
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateFarmer(int farmerId)
        {
            try
            {
                await _farmerService.DeactivateFarmerAsync(farmerId);

                // Get the farmer name for the success message
                var farmer = await _farmerService.GetFarmerByIdAsync(farmerId);
                TempData["SuccessMessage"] = $"Farmer {farmer.User?.FirstName} {farmer.User?.LastName} has been deactivated.";

                return RedirectToAction(nameof(ManageFarmers));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deactivating farmer: {ex.Message}";
                return RedirectToAction(nameof(ManageFarmers));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReactivateFarmer(int farmerId)
        {
            try
            {
                await _farmerService.ReactivateFarmerAsync(farmerId);

                // Get the farmer name for the success message
                var farmer = await _farmerService.GetFarmerByIdAsync(farmerId);
                TempData["SuccessMessage"] = $"Farmer {farmer.User?.FirstName} {farmer.User?.LastName} has been reactivated.";

                return RedirectToAction(nameof(ManageFarmers));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error reactivating farmer: {ex.Message}";
                return RedirectToAction(nameof(ManageFarmers));
            }
        }

        private async Task<IActionResult> ToggleProductStatus(int productId, Func<int, Task> toggleAction, string successMessage, string errorPrefix, string returnUrl = null)
        {
            try
            {
                // Get the product name before changing status (for the success message)
                var product = await _productService.GetProductByIdAsync(productId);
                if (product == null)
                    return NotFound();

                // Apply the toggle action (activate or deactivate)
                await toggleAction(productId);

                TempData["SuccessMessage"] = string.Format(successMessage, product.Name);

                // Redirect back to the referring page if specified
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                // Default redirect to products page
                return RedirectToAction(nameof(Products));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"{errorPrefix}: {ex.Message}";

                // Redirect back to the referring page if specified
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                // Default redirect to products page
                return RedirectToAction(nameof(Products));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateProduct(int productId, string returnUrl = null)
        {
            return await ToggleProductStatus(
                productId,
                _productService.DeactivateProductAsync,
                "Product '{0}' has been marked as out of stock.",
                "Error deactivating product",
                returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateProduct(int productId, string returnUrl = null)
        {
            return await ToggleProductStatus(
                productId,
                _productService.ActivateProductAsync,
                "Product '{0}' has been marked as in stock.",
                "Error activating product",
                returnUrl);
        }
    }
}