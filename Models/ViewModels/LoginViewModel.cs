using System.ComponentModel.DataAnnotations;

namespace AgriEnergyConnect.Models.ViewModels
{
    // The LoginViewModel class is a ViewModel used to transfer user login data
    // between the Razor Pages views and the application layers.
    // It includes validation attributes to ensure data integrity and user-friendly error messages.
    public class LoginViewModel
    {
        // The username of the user attempting to log in.
        // This is a required field, and validation ensures that the username is provided.
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        // The password of the user attempting to log in.
        // This is a required field, and validation ensures that the password is provided.
        // The DataType attribute specifies that this field should be treated as a password.
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // Indicates whether the user wants to remain logged in across sessions.
        // This is an optional field and defaults to false if not specified.
        public bool RememberMe { get; set; }

        // The URL to redirect the user to after a successful login.
        // This is an optional field and is typically used to return the user to their original destination.
        public string ReturnUrl { get; set; }
    }
}