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

        // Constructor for the EmployeeController class.
        // Parameters:
        //   farmerService - The service for managing farmers.
        //   productService - The service for managing products.
        public EmployeeController(IFarmerService farmerService, IProductService productService, IAuthService authService)
        {
            _farmerService = farmerService;
            _productService = productService;
            _authService = authService;
        }

        // Displays the employee dashboard with summary data.
        // Retrieves total farmers, total products, recent farmers, and active users.
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var user = await _authService.GetUserByIdAsync(userId);

                if (user == null)
                    return NotFound();

                // Set ViewBag employee data
                ViewBag.Employee = new { User = user };

                // Get all farmers and products for statistics
                var farmers = await _farmerService.GetAllFamersAsync();
                var farmerList = farmers.ToList();
                var products = await _productService.GetAllProductsAsync();
                var productList = products.ToList();
                var allUsers = await _authService.GetAllUsersAsync();
                var userList = allUsers.ToList();

                // Calculate statistics
                var totalFarmers = farmerList.Count;
                var totalProducts = productList.Count;
                var activeUsers = userList.Count(u => u.IsActive);
                var totalUsers = userList.Count;

                // Get all farmers for "Recent Farmers" section (not just recent additions)
                var recentFarmers = farmerList
                    .OrderByDescending(f => f.User?.CreatedDate ?? DateTime.MinValue)
                    .Take(4)
                    .ToList();

                // Get recent products (last 7 days)
                var recentProducts = productList
                    .Where(p => p.CreatedDate >= DateTime.Now.AddDays(-7))
                    .GroupBy(p => p.FarmerId)
                    .Select(g => new { FarmerId = g.Key, Count = g.Count(), LastUpdate = g.Max(p => p.CreatedDate) })
                    .ToList();

                // Set ViewBag statistics
                ViewBag.TotalFarmers = totalFarmers;
                ViewBag.TotalProducts = totalProducts;
                ViewBag.ActiveUsers = activeUsers;
                ViewBag.NewFarmers = recentFarmers.Count(f => f.User?.CreatedDate >= DateTime.Now.AddDays(-7));
                ViewBag.NewProducts = recentProducts.Sum(p => p.Count);
                ViewBag.ActivePercentage = totalUsers > 0 ? Math.Round((decimal)activeUsers / totalUsers * 100, 1) : 0;

                // Create recent activities using the static methods
                var activities = new List<ActivityItem>();

                // Add recent farmer additions (for the past week)
                var newFarmers = recentFarmers.Where(f => f.User?.CreatedDate >= DateTime.Now.AddDays(-7)).Take(1);
                foreach (var farmer in newFarmers)
                {
                    if (farmer.User != null)
                    {
                        activities.Add(ActivityItem.NewFarmerAdded(
                            $"{farmer.User.FirstName} {farmer.User.LastName}",
                            farmer.User.CreatedDate
                        ));
                    }
                }

                // Add recent product updates
                foreach (var productGroup in recentProducts.Take(1))
                {
                    var farmer = farmerList.FirstOrDefault(f => f.FarmerId == productGroup.FarmerId);
                    if (farmer?.User != null)
                    {
                        activities.Add(ActivityItem.ProductUpdate(
                            $"{farmer.User.FirstName} {farmer.User.LastName}",
                            productGroup.Count,
                            productGroup.LastUpdate
                        ));
                    }
                }

                // Add a sample report activity
                activities.Add(ActivityItem.ReportGenerated(
                    "Monthly product",
                    DateTime.Now.AddDays(-1).AddHours(-7).AddMinutes(-20)
                ));

                // Create the view model
                var viewModel = new EmployeeDashboardViewModel
                {
                    Employee = user,
                    TotalFarmers = totalFarmers,
                    TotalProducts = totalProducts,
                    ActiveUsers = activeUsers,
                    NewFarmers = ViewBag.NewFarmers,
                    NewProducts = ViewBag.NewProducts,
                    ActivePercentage = ViewBag.ActivePercentage,
                    // Map farmers to FarmerSummaryDTO ensuring to include the user details
                    RecentFarmers = recentFarmers.Select(f => new FarmerSummaryDTO
                    {
                        FarmerId = f.FarmerId,
                        FarmName = f.FarmName,
                        Location = f.Location,
                        OwnerName = f.User != null ? $"{f.User.FirstName ?? "Unknown"} {f.User.LastName ?? "Farmer"}".Trim() : "Unknown Farmer",
                        ProductCount = f.Products?.Count ?? 0
                    }).ToList(),
                    RecentActivities = activities
                };

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
                // Create user
                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    Role = UserRole.Farmer,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };

                // Create farmer
                var farmer = new Farmer
                {
                    FarmName = model.FarmName,
                    Location = model.Location,
                    User = user
                };

                await _farmerService.AddFarmerAsync(farmer, model.Password);

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
        public async Task<IActionResult> ViewFarmerProducts(int farmerId)
        {
            var farmer = await _farmerService.GetFarmerByIdAsync(farmerId);

            if (farmer == null)
                return NotFound();

            ViewBag.Farmer = farmer;

            var products = await _productService.GetProductsByFarmerIdAsync(farmerId);
            return View(products);
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
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _authService.GetUserByIdAsync(userId);

            if (user == null)
                return NotFound();

            // Set ViewBag employee data
            ViewBag.Employee = new { User = user };

            var farmerDTO = await _farmerService.GetFarmerDTOByIdAsync(farmerId);
            if (farmerDTO == null)
                return NotFound();

            return View(farmerDTO);
        }

        // Displays detailed information about a specific product.
        // Parameters:
        //   productId - The ID of the product whose details are to be displayed.
        [HttpGet]
        public async Task<IActionResult> ProductDetails(int productId)
        {
            var productDTO = await _productService.GetProductDTOByIdAsync(productId);
            if (productDTO == null)
                return NotFound();
            return View(productDTO);
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
        public async Task<IActionResult> ManageFarmers(string searchTerm = "", string locationFilter = "", int page = 1, int pageSize = 10)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _authService.GetUserByIdAsync(userId);

            if (user == null)
                return NotFound();

            // Set ViewBag employee data
            ViewBag.Employee = new { User = user };

            // Get all farmers with their complete information
            var farmers = await _farmerService.GetAllFamersAsync();
            var farmerList = farmers.ToList();

            // Create farmer summaries with correct IsActive and ProductCount
            var farmerSummaries = farmerList.Select(farmer => new FarmerSummaryDTO
            {
                FarmerId = farmer.FarmerId,
                FarmName = farmer.FarmName,
                Location = farmer.Location,
                OwnerName = farmer.User != null
                    ? $"{farmer.User.FirstName} {farmer.User.LastName}".Trim()
                    : "Unknown Farmer",
                ProductCount = farmer.Products?.Count ?? 0,
                IsActive = farmer.User?.IsActive ?? false // This is the fix for IsActive
            }).ToList();

            // Apply filters
            if (!string.IsNullOrEmpty(searchTerm))
            {
                farmerSummaries = farmerSummaries.Where(f =>
                    f.OwnerName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    f.FarmName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    f.Location.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            if (!string.IsNullOrEmpty(locationFilter))
            {
                farmerSummaries = farmerSummaries.Where(f =>
                    f.Location.Equals(locationFilter, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            // Get unique locations for the filter dropdown
            var uniqueLocations = farmerSummaries.Select(f => f.Location)
                .Distinct()
                .OrderBy(l => l)
                .ToList();

            // Implement pagination
            var totalFarmers = farmerSummaries.Count;
            var totalPages = (int)Math.Ceiling(totalFarmers / (double)pageSize);
            var paginatedFarmers = farmerSummaries
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Set ViewBag data for the view
            ViewBag.SearchTerm = searchTerm;
            ViewBag.SelectedLocation = locationFilter;
            ViewBag.UniqueLocations = uniqueLocations;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalFarmers = totalFarmers;

            return View(paginatedFarmers);
        }

        [HttpGet]
        public async Task<IActionResult> EditFarmer(int farmerId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _authService.GetUserByIdAsync(userId);

            if (user == null)
                return NotFound();

            // Set ViewBag employee data
            ViewBag.Employee = new { User = user };

            var farmer = await _farmerService.GetFarmerByIdAsync(farmerId);
            if (farmer == null)
                return NotFound();

            var viewModel = new FarmerViewModel
            {
                FarmName = farmer.FarmName,
                Location = farmer.Location,
                FirstName = farmer.User.FirstName,
                LastName = farmer.User.LastName,
                Email = farmer.User.Email,
                PhoneNumber = farmer.User.PhoneNumber,
                Username = farmer.User.Username
            };

            ViewBag.FarmerId = farmerId;
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFarmer(int farmerId, FarmerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.FarmerId = farmerId;
                return View(model);
            }

            try
            {
                var farmer = await _farmerService.GetFarmerByIdAsync(farmerId);
                if (farmer == null)
                    return NotFound();

                // Update farmer information
                farmer.FarmName = model.FarmName;
                farmer.Location = model.Location;
                farmer.User.FirstName = model.FirstName;
                farmer.User.LastName = model.LastName;
                farmer.User.Email = model.Email;
                farmer.User.PhoneNumber = model.PhoneNumber;
                farmer.User.Username = model.Username;

                // Only update password if a new one is provided
                if (!string.IsNullOrEmpty(model.Password))
                {
                    var hasher = new PasswordHasher<User>();
                    farmer.User.PasswordHash = hasher.HashPassword(farmer.User, model.Password);
                }

                await _farmerService.UpdateFarmerAsync(farmer);

                TempData["SuccessMessage"] = $"Farmer {model.FirstName} {model.LastName} was successfully updated.";
                return RedirectToAction(nameof(ManageFarmers));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred while updating the farmer: {ex.Message}");
                ViewBag.FarmerId = farmerId;
                return View(model);
            }
        }
    }
}