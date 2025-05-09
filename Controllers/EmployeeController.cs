using AgriEnergyConnect.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AgriEnergyConnect.Models.ViewModels;
using AgriEnergyConnect.Models;
using AgriEnergyConnect.DTOs;

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

        // Constructor for the EmployeeController class.
        // Parameters:
        //   farmerService - The service for managing farmers.
        //   productService - The service for managing products.
        public EmployeeController(IFarmerService farmerService, IProductService productService)
        {
            _farmerService = farmerService;
            _productService = productService;
        }

        // Displays the employee dashboard with summary data.
        // Retrieves total farmers, total products, recent farmers, and active users.
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var farmers = await _farmerService.GetAllFamersAsync();
            var farmerSummaries = await _farmerService.GetAllFarmerSummariesAsync();
            var products = await _productService.GetAllProductsAsync();

            ViewBag.TotalFarmers = farmers.Count();
            ViewBag.TotalProducts = products.Count();
            ViewBag.RecentProducts = farmers.OrderByDescending(f => f.User.CreatedDate).Take(4).ToList();

            // Calculate active users (this is for demonstration purposes)
            ViewBag.ActiveUsers = farmers.Count(f => f.User.IsActive);

            return View();
        }

        // Displays the form for adding a new farmer.
        [HttpGet]
        public IActionResult AddFarmer()
        {
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
        public async Task<IActionResult> ManageFarmers()
        {
            var farmers = await _farmerService.GetAllFarmerSummariesAsync();
            return View(farmers);
        }
    }
}