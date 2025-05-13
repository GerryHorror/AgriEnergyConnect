using AgriEnergyConnect.Models;
using AgriEnergyConnect.Models.ViewModels;

namespace AgriEnergyConnect.Services.Interfaces
{
    // The IRegistrationRequestService interface defines the contract for managing registration requests in the system.
    // It provides methods for retrieving, adding, updating, and handling the approval/rejection of registration requests.
    public interface IRegistrationRequestService
    {
        // Retrieves all pending registration requests from the database.
        // Returns a collection of RegistrationRequestViewModel objects for display in the UI.
        Task<IEnumerable<RegistrationRequestViewModel>> GetPendingRequestsAsync();

        // Retrieves a specific registration request by its unique ID (RequestId).
        // Parameters:
        //   requestId - The ID of the registration request to retrieve.
        // Returns a RegistrationRequestViewModel containing the request details, or null if it doesn't exist.
        Task<RegistrationRequestViewModel> GetRequestByIdAsync(int requestId);

        // Creates a new registration request from a RegisterViewModel.
        // Parameters:
        //   model - The RegisterViewModel containing the user's registration details.
        // Returns the newly created RegistrationRequest object.
        Task<RegistrationRequest> CreateRequestFromViewModelAsync(RegisterViewModel model);

        // Approves a registration request and creates a new user and farmer account.
        // Parameters:
        //   requestId - The ID of the registration request to approve.
        // Returns the newly created User object for the approved farmer.
        Task<User> ApproveRequestAsync(int requestId);

        // Rejects a registration request with a specified reason.
        // Parameters:
        //   requestId - The ID of the registration request to reject.
        //   reason - The reason for rejecting the request.
        // Returns true if the rejection was successful, or false if the request doesn't exist.
        Task<bool> RejectRequestAsync(int requestId, string reason);

        // Checks if a registration request exists for a specific username or email.
        // Parameters:
        //   username - The username to check.
        //   email - The email to check.
        // Returns true if a request exists for the given username or email, or false otherwise.
        Task<bool> RequestExistsAsync(string username, string email);

        // Sends a notification email to a user regarding their registration request.
        // Parameters:
        //   email - The email address of the user to notify.
        //   isApproved - Whether the request was approved or rejected.
        //   reason - The reason for rejection (only applicable if isApproved is false).
        // Returns true if the notification was sent successfully, or false otherwise.
        Task<bool> SendRegistrationNotificationAsync(string email, bool isApproved, string reason = null);
    }
}