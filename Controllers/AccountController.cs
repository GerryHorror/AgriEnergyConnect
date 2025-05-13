using System.Security.Claims;
using AgriEnergyConnect.Models;
using AgriEnergyConnect.Models.ViewModels;
using AgriEnergyConnect.Services.Implementation;
using AgriEnergyConnect.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriEnergyConnect.Controllers
{
    // The AccountController class handles user authentication, registration, and account-related actions.
    // It provides methods for logging in, logging out, registering, and handling access denial.
    public class AccountController : Controller
    {
        // Services for authentication, farmer management, and registration requests.

        private readonly IAuthService _authService;
        private readonly IFarmerService _farmerService;
        private readonly IRegistrationRequestService _registrationRequestService;

        // Constructor for the AccountController class.
        // Parameters:
        //   authService - The service for managing user authentication.
        //   farmerService - The service for managing farmers.
        //   registrationRequestService - The service for managing registration requests.
        public AccountController(
            IAuthService authService,
            IFarmerService farmerService,
            IRegistrationRequestService registrationRequestService)
        {
            _authService = authService;
            _farmerService = farmerService;
            _registrationRequestService = registrationRequestService;
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

            try
            {
                // Authenticate the user
                var user = await _authService.AuthenticateAsync(model.Username, model.Password);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }

                // Create claims identity
                var claimsIdentity = _authService.CreateUserClaims(user);

                // Setup authentication properties
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(12)
                };

                // Sign in the user
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // Redirect based on role
                if (user.Role == UserRole.Farmer)
                {
                    return RedirectToAction("Dashboard", "Farmer");
                }
                else
                {
                    return RedirectToAction("Dashboard", "Employee");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Login error: {ex.Message}");
                return View(model);
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
        // For farmer registrations, creates a registration request for admin approval.
        // For employee registrations, creates the account directly (if called by an admin).
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
                if (model.IsFarmer)
                {
                    // For farmers, create a registration request that requires admin approval
                    if (string.IsNullOrEmpty(model.FarmName) || string.IsNullOrEmpty(model.Location))
                    {
                        ModelState.AddModelError(string.Empty, "Farm name and location are required for farmer registration.");
                        return View(model);
                    }

                    // Check if username or email already exists
                    if (await _authService.UsernameExistsAsync(model.Username))
                    {
                        ModelState.AddModelError(string.Empty, "Username already exists. Please choose a different username.");
                        return View(model);
                    }

                    if (await _authService.EmailExistsAsync(model.Email))
                    {
                        ModelState.AddModelError(string.Empty, "Email already exists. Please use a different email address.");
                        return View(model);
                    }

                    // Create a registration request
                    await _registrationRequestService.CreateRequestFromViewModelAsync(model);

                    // Success, redirect to login with message
                    TempData["SuccessMessage"] = "Registration request submitted. An administrator will review your application. You will be notified by email when your account is approved.";
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    // For employees, register directly (this should be restricted to admin users in a real app)
                    if (User.IsInRole("Employee"))
                    {
                        await _authService.RegisterUserAsync(model);
                        TempData["SuccessMessage"] = "Registration successful. The new employee can now login.";
                        return RedirectToAction(nameof(Login));
                    }
                    else
                    {
                        // Non-admins cannot create employee accounts
                        ModelState.AddModelError(string.Empty, "Only existing administrators can create employee accounts.");
                        return View(model);
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                // Handle specific exceptions like duplicate username/email
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                ModelState.AddModelError(string.Empty, $"Registration failed: {ex.Message}");
                return View(model);
            }
        }

        // Displays the list of pending registration requests for admin approval.
        // This action is restricted to users with the "Employee" role.
        [HttpGet]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> RegistrationRequests()
        {
            var requests = await _registrationRequestService.GetPendingRequestsAsync();
            return View(requests);
        }

        // Approves a registration request and creates a new farmer account.
        // Parameters:
        //   requestId - The ID of the registration request to approve.
        // Returns a redirect to the registration requests page.
        [HttpPost]
        [Authorize(Roles = "Employee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveRequest(int requestId)
        {
            try
            {
                var user = await _registrationRequestService.ApproveRequestAsync(requestId);
                TempData["SuccessMessage"] = $"Registration for {user.FirstName} {user.LastName} has been approved. They can now log in to the system.";
            }
            catch (KeyNotFoundException)
            {
                TempData["ErrorMessage"] = "Registration request not found.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error approving registration: {ex.Message}";
            }

            return RedirectToAction(nameof(RegistrationRequests));
        }

        // Rejects a registration request.
        // Parameters:
        //   requestId - The ID of the registration request to reject.
        //   rejectionReason - The reason for rejecting the request.
        // Returns a redirect to the registration requests page.
        [HttpPost]
        [Authorize(Roles = "Employee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectRequest(int requestId, string rejectionReason)
        {
            if (string.IsNullOrWhiteSpace(rejectionReason))
            {
                TempData["ErrorMessage"] = "A reason for rejection is required.";
                return RedirectToAction(nameof(RegistrationRequests));
            }

            try
            {
                var result = await _registrationRequestService.RejectRequestAsync(requestId, rejectionReason);

                if (result)
                {
                    TempData["SuccessMessage"] = "Registration request has been rejected. The applicant will be notified.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Registration request not found.";
                }
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error rejecting registration: {ex.Message}";
            }

            return RedirectToAction(nameof(RegistrationRequests));
        }
    }
}