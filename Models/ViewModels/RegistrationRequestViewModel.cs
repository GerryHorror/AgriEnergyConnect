namespace AgriEnergyConnect.Models.ViewModels
{
    // The RegistrationRequestViewModel class is a ViewModel used to transfer registration request data
    // between the Razor Pages views and the application layers.
    public class RegistrationRequestViewModel
    {
        // The unique identifier of the registration request.
        public int RequestId { get; set; }

        // User account information

        // The username chosen by the user requesting registration.
        public string Username { get; set; }

        // Personal information

        // The first name of the user requesting registration.
        public string FirstName { get; set; }

        // The last name of the user requesting registration.
        public string LastName { get; set; }

        // The email address of the user requesting registration.
        public string Email { get; set; }

        // The phone number of the user requesting registration.
        public string PhoneNumber { get; set; }

        // Farm information

        // The name of the farm associated with the user requesting registration.
        public string FarmName { get; set; }

        // The location of the farm associated with the user requesting registration.
        public string Location { get; set; }

        // Request metadata

        // The date and time when the registration request was submitted.
        public DateTime RequestDate { get; set; }

        // The status of the registration request.
        public string Status { get; set; }
    }
}