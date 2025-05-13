using System.ComponentModel.DataAnnotations;

namespace AgriEnergyConnect.Models
{
    // The RegistrationRequest class represents a pending registration request in the system.
    // It stores information about a farmer who has requested to join the platform.
    public class RegistrationRequest
    {
        // Primary key for the RegistrationRequest entity.
        // This uniquely identifies each registration request in the database.
        [Key]
        public int RequestId { get; set; }

        // User account information

        // The username chosen by the user requesting registration.
        // This is a required field with a maximum length of 100 characters.
        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        // The hashed password of the user requesting registration.
        // This is a required field with a maximum length of 100 characters.
        // Storing the password as a hash ensures security.
        [Required]
        [StringLength(100)]
        public string PasswordHash { get; set; }

        // Personal information

        // The first name of the user requesting registration.
        // This is a required field.
        [Required]
        public string FirstName { get; set; }

        // The last name of the user requesting registration.
        // This is a required field.
        [Required]
        public string LastName { get; set; }

        // The email address of the user requesting registration.
        // This is a required field and must follow a valid email format.
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        // The phone number of the user requesting registration.
        // This is a required field.
        [Required]
        public string PhoneNumber { get; set; }

        // Farm information

        // The name of the farm associated with the user requesting registration.
        // This is a required field.
        [Required]
        public string FarmName { get; set; }

        // The location of the farm associated with the user requesting registration.
        // This is a required field.
        [Required]
        public string Location { get; set; }

        // Request metadata

        // The date and time when the registration request was submitted.
        // Defaults to the current date and time when a new request is created.
        public DateTime RequestDate { get; set; } = DateTime.Now;

        // The status of the registration request.
        // This can be "Pending", "Approved", or "Rejected".
        public string Status { get; set; } = "Pending";

        // The reason for rejection if the request is rejected.
        // This is null if the request is not rejected.
        public string? RejectionReason { get; set; }
    }
}