using System.Security.Claims;
using AgriEnergyConnect.Models;
using AgriEnergyConnect.Models.ViewModels;
using AgriEnergyConnect.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace AgriEnergyConnect.Controllers
{
    // The AccountController class handles user authentication, registration, and account-related actions.
    // It provides methods for logging in, logging out, registering, and handling access denial.
    public class AccountController : Controller
    {
        // Services for authentication and farmer management.
        private readonly IAuthService _authService;

        private readonly IFarmerService _farmerService;

        // Constructor for the AccountController class.
        // Parameters:
        //   authService - The service for managing user authentication.
        //   farmerService - The service for managing farmers.
        public AccountController(IAuthService authService, IFarmerService farmerService)
        {
            _authService = authService;
            _farmerService = farmerService;
        }

        // Displays the login page.
        // Parameters:
        //   returnUrl - The URL to redirect the user to after a successful login (optional).
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            // Clear any existing authentication cookies
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        // Handles the submission of the login form.
        // Parameters:
        //   model - The LoginViewModel containing the user's login credentials.
        // Returns a redirect to the appropriate dashboard on success or redisplays the login form on failure.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _authService.AuthenticateAsync(model.Username, model.Password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            // Create claims for the authenticated user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(12)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            // Redirect the user based on their role
            if (user.Role == UserRole.Farmer)
            {
                return RedirectToAction("Dashboard", "Farmer");
            }
            else
            {
                return RedirectToAction("Dashboard", "Employee");
            }
        }

        // Logs the user out and redirects to the login page.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        // Displays the access denied page.
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // Displays the registration page.
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        // Handles the submission of the registration form.
        // Parameters:
        //   model - The RegisterViewModel containing the user's registration details.
        // Returns a redirect to the login page on success or redisplays the registration form on failure.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                // Create the user
                var user = new User
                {
                    Username = model.Username,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Role = model.IsFarmer ? UserRole.Farmer : UserRole.Employee,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                if (model.IsFarmer)
                {
                    // Create the farmer with the associated user
                    var farmer = new Farmer
                    {
                        FarmName = model.FarmName,
                        Location = model.Location
                    };

                    await _farmerService.AddFarmerAsync(farmer, model.Password);
                }
                else
                {
                    // For employees (not supported through public registration)
                    ModelState.AddModelError(string.Empty, "Employee registration is not supported through public registration");
                    return View(model);
                }

                // Success, redirect to login
                TempData["SuccessMessage"] = "Registration successful. You can now login.";
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., user already exists)
                ModelState.AddModelError(string.Empty, $"Registration failed: {ex.Message}");
                return View(model);
            }
        }
    }
}