using AgriEnergyConnect.Models;
using AgriEnergyConnect.Models.ViewModels;
using AgriEnergyConnect.Repositories.Interfaces;
using AgriEnergyConnect.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AgriEnergyConnect.Services.Implementation
{
    // The RegistrationRequestService class implements the IRegistrationRequestService interface and provides the business logic
    // for managing registration requests in the system. It interacts with the repositories to perform CRUD operations
    // and handles the approval/rejection process for farmer registration requests.
    public class RegistrationRequestService : IRegistrationRequestService
    {
        private readonly IRegistrationRequestRepository _requestRepository;
        private readonly IFarmerRepository _farmerRepository;
        private readonly IFarmerService _farmerService;
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();
        private readonly IMessageService _messageService;  // For sending system notifications
        private readonly IAuthService _authService; // Added this to find admin users

        // Constructor for the RegistrationRequestService class.
        // Parameters:
        //   requestRepository - The repository for registration request operations.
        //   farmerRepository - The repository for farmer operations.
        //   farmerService - The service for farmer operations.
        //   messageService - The service for message operations.
        //   authService - The service for authentication operations.
        public RegistrationRequestService(
            IRegistrationRequestRepository requestRepository,
            IFarmerRepository farmerRepository,
            IFarmerService farmerService,
            IMessageService messageService,
            IAuthService authService)
        {
            _requestRepository = requestRepository;
            _farmerRepository = farmerRepository;
            _farmerService = farmerService;
            _messageService = messageService;
            _authService = authService; // Initialize the auth service
        }

        // Retrieves all pending registration requests from the database.
        // Returns a collection of RegistrationRequestViewModel objects for display in the UI.
        public async Task<IEnumerable<RegistrationRequestViewModel>> GetPendingRequestsAsync()
        {
            var requests = await _requestRepository.GetPendingRequestsAsync();

            // Map the RegistrationRequest entities to RegistrationRequestViewModel objects
            return requests.Select(r => new RegistrationRequestViewModel
            {
                RequestId = r.RequestId,
                Username = r.Username,
                FirstName = r.FirstName,
                LastName = r.LastName,
                Email = r.Email,
                PhoneNumber = r.PhoneNumber,
                FarmName = r.FarmName,
                Location = r.Location,
                RequestDate = r.RequestDate,
                Status = r.Status
            });
        }

        // Retrieves a specific registration request by its unique ID (RequestId).
        // Parameters:
        //   requestId - The ID of the registration request to retrieve.
        // Returns a RegistrationRequestViewModel containing the request details, or null if it doesn't exist.
        public async Task<RegistrationRequestViewModel> GetRequestByIdAsync(int requestId)
        {
            var request = await _requestRepository.GetRequestByIdAsync(requestId);

            if (request == null)
                return null;

            // Map the RegistrationRequest entity to a RegistrationRequestViewModel
            return new RegistrationRequestViewModel
            {
                RequestId = request.RequestId,
                Username = request.Username,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                FarmName = request.FarmName,
                Location = request.Location,
                RequestDate = request.RequestDate,
                Status = request.Status
            };
        }

        // Creates a new registration request from a RegisterViewModel.
        // Parameters:
        //   model - The RegisterViewModel containing the user's registration details.
        // Returns the newly created RegistrationRequest object.
        public async Task<RegistrationRequest> CreateRequestFromViewModelAsync(RegisterViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            // Check if a request already exists for this username or email
            if (await RequestExistsAsync(model.Username, model.Email))
                throw new InvalidOperationException("A registration request with this username or email already exists.");

            // Create a temporary user to hash the password
            var tempUser = new User { Username = model.Username };
            var hashedPassword = _passwordHasher.HashPassword(tempUser, model.Password);

            // Create the registration request
            var request = new RegistrationRequest
            {
                Username = model.Username,
                PasswordHash = hashedPassword,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                FarmName = model.FarmName,
                Location = model.Location,
                RequestDate = DateTime.Now,
                Status = "Pending"
            };

            // Save the request to the database
            await _requestRepository.AddRequestAsync(request);

            return request;
        }

        // Approves a registration request and creates a new user and farmer account.
        // Parameters:
        //   requestId - The ID of the registration request to approve.
        // Returns the newly created User object for the approved farmer.
        public async Task<User> ApproveRequestAsync(int requestId)
        {
            var request = await _requestRepository.GetRequestByIdAsync(requestId);

            if (request == null)
                throw new KeyNotFoundException($"Registration request with ID {requestId} not found.");

            if (request.Status != "Pending")
                throw new InvalidOperationException("This registration request has already been processed.");

            // Create the user entity
            var user = new User
            {
                Username = request.Username,
                PasswordHash = request.PasswordHash, // Already hashed during request creation
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Role = UserRole.Farmer,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            // Create the farmer entity
            var farmer = new Farmer
            {
                FarmName = request.FarmName,
                Location = request.Location,
                User = user
            };

            // Add the farmer to the database using the farmer service
            await _farmerRepository.AddFarmerAsync(farmer);

            // Update the request status
            request.Status = "Approved";
            await _requestRepository.UpdateRequestAsync(request);

            // Send approval notification
            await SendRegistrationNotificationAsync(request.Email, true);

            try
            {
                // Find an admin user to send the welcome message from
                var allUsers = await _authService.GetAllUsersAsync();
                var adminUser = allUsers.FirstOrDefault(u => u.Role == UserRole.Employee);

                if (adminUser != null)
                {
                    // Create a welcome message for the farmer
                    await _messageService.SendMessageAsync(
                        adminUser.UserId,
                        user.UserId,
                        "Welcome to Agri-Energy Connect",
                        $"Dear {user.FirstName},\n\nYour registration request has been approved. Welcome to the Agri-Energy Connect platform!\n\nYou can now log in and start using all the features available to farmers on our platform.\n\nIf you have any questions or need assistance, please don't hesitate to contact us.\n\nBest regards,\nThe Agri-Energy Connect Team"
                    );
                }
                // If no admin user is found, we skip sending the welcome message
                // You might want to add some logging here in a production environment
            }
            catch (Exception ex)
            {
                // Log the error, but don't prevent the user from being created
                Console.WriteLine($"Error sending welcome message: {ex.Message}");
                // In a production environment, you'd want to log this to a proper logging system
            }

            return user;
        }

        // Rejects a registration request with a specified reason.
        // Parameters:
        //   requestId - The ID of the registration request to reject.
        //   reason - The reason for rejecting the request.
        // Returns true if the rejection was successful, or false if the request doesn't exist.
        public async Task<bool> RejectRequestAsync(int requestId, string reason)
        {
            var request = await _requestRepository.GetRequestByIdAsync(requestId);

            if (request == null)
                return false;

            if (request.Status != "Pending")
                throw new InvalidOperationException("This registration request has already been processed.");

            // Update the request status and rejection reason
            request.Status = "Rejected";
            request.RejectionReason = reason;
            await _requestRepository.UpdateRequestAsync(request);

            // Send rejection notification
            await SendRegistrationNotificationAsync(request.Email, false, reason);

            return true;
        }

        // Checks if a registration request exists for a specific username or email.
        // Parameters:
        //   username - The username to check.
        //   email - The email to check.
        // Returns true if a request exists for the given username or email, or false otherwise.
        public async Task<bool> RequestExistsAsync(string username, string email)
        {
            return await _requestRepository.RequestExistsAsync(username, email);
        }

        // Sends a notification email to a user regarding their registration request.
        // Parameters:
        //   email - The email address of the user to notify.
        //   isApproved - Whether the request was approved or rejected.
        //   reason - The reason for rejection (only applicable if isApproved is false).
        // Returns true if the notification was sent successfully, or false otherwise.
        public async Task<bool> SendRegistrationNotificationAsync(string email, bool isApproved, string reason = null)
        {
            // In a real application, this would send an actual email
            // For now, we'll just return true to simulate successful sending

            // In a real implementation, you would use a service like SendGrid, SMTP, or another email service
            // to send actual emails to users

            Console.WriteLine($"Registration notification sent to {email}. Status: {(isApproved ? "Approved" : "Rejected")}");
            if (!isApproved && !string.IsNullOrEmpty(reason))
            {
                Console.WriteLine($"Rejection reason: {reason}");
            }

            return true;
        }
    }
}