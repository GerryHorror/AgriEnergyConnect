using System.ComponentModel.DataAnnotations;

namespace AgriEnergyConnect.Models.ViewModels
{
    // The RegisterViewModel class is a ViewModel used to transfer user registration data
    // between the Razor Pages views and the application layers.
    // It includes validation attributes to ensure data integrity and user-friendly error messages.
    public class RegisterViewModel
    {
        // User account information

        // The username for the new user account.
        // This is a required field with a minimum length of 4 and a maximum length of 50 characters.
        // Validation ensures that the username is provided and meets the length requirements.
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Username must be between 4 and 50 characters.")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        // The password for the new user account.
        // This is a required field with a minimum length of 6 characters.
        // Validation ensures that the password is provided and meets the length requirements.
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        // The confirmation password for the new user account.
        // This field must match the value of the Password field.
        // Validation ensures that the passwords match and provides a user-friendly error message if they do not.
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        // Personal information

        // The first name of the user.
        // This is a required field with a maximum length of 50 characters.
        // Validation ensures that the first name is provided and does not exceed the character limit.
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        // The last name of the user.
        // This is a required field with a maximum length of 50 characters.
        // Validation ensures that the last name is provided and does not exceed the character limit.
        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        // The email address of the user.
        // This is a required field and must follow a valid email format.
        // Validation ensures that the email is provided and is in a valid format.
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        // The phone number of the user.
        // This is a required field and must follow a valid phone number format.
        // Validation ensures that the phone number is provided and is in a valid format.
        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        // For farmers only

        // The name of the farm associated with the user.
        // This is an optional field and is only applicable if the user is registering as a farmer.
        [Display(Name = "Farm Name")]
        public string FarmName { get; set; }

        // The location of the farm associated with the user.
        // This is an optional field and is only applicable if the user is registering as a farmer.
        [Display(Name = "Location")]
        public string Location { get; set; }

        // Determines if the user is registering as a farmer.
        // This is a boolean field that indicates whether the user is a farmer.
        [Display(Name = "Register as Farmer")]
        public bool IsFarmer { get; set; }
    }
}