using AgriEnergyConnect.Validation;
using System.ComponentModel.DataAnnotations;

namespace AgriEnergyConnect.Models.ViewModels
{
    // The FarmerViewModel class is a ViewModel used to transfer farmer registration data
    // between the Razor Pages views and the application layers.
    // It includes validation attributes to ensure data integrity and user-friendly error messages.
    public class FarmerViewModel
    {
        // User details

        // The first name of the farmer.
        // This is a required field, and validation ensures that the first name is provided.
        [Required(ErrorMessage = "First name is required")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        // The last name of the farmer.
        // This is a required field, and validation ensures that the last name is provided.
        [Required(ErrorMessage = "Last name is required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        // The email address of the farmer.
        // This is a required field and must follow a valid email format.
        // Validation ensures that the email is provided and is in a valid format.
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        // The phone number of the farmer.
        // This is a required field and must follow a valid phone number format.
        // Validation ensures that the phone number is provided and is in a valid format.
        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        // Farm details

        // The name of the farm associated with the farmer.
        // This is a required field, and validation ensures that the farm name is provided.
        [Required(ErrorMessage = "Farm name is required")]
        [Display(Name = "Farm Name")]
        public string FarmName { get; set; }

        // The location of the farm associated with the farmer.
        // This is a required field, and validation ensures that the farm location is provided.
        [Required(ErrorMessage = "Farm location is required")]
        [Display(Name = "Farm Location")]
        public string Location { get; set; }

        // Account details

        // The username for the farmer's account.
        // This is a required field, and validation ensures that the username is provided.
        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        // The password for the farmer's account.
        // Only required during registration, not when editing.
        [RequiredForRegistration(ContextKey = "FormContext")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}