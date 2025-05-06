using System.ComponentModel.DataAnnotations;

namespace AgriEnergyConnect.Models
{
    // This enum defines the different roles a user can have in the system.
    // It helps categorise users based on their responsibilities snd permissions.
    public enum UserRole
    {
        Farmer,    // Represents a user who is a farmer.
        Employee   // Represents a user who is an employee of the system.
    }

    // The User class represents a user in the AgriEnergyConnect system.
    // It contains properties that store user-related information and metadata.
    public class User
    {
        // Primary key for the User entity.
        // This uniquely identifies each user in the database.
        [Key]
        public int UserId { get; set; }

        // The username of the user.
        // This is a required field with a maximum length of 100 characters.
        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        // The hashed password of the user.
        // This is a required field with a maximum length of 100 characters.
        // Storing the password as a hash ensures security.
        [Required]
        [StringLength(100)]
        public string PasswordHash { get; set; }

        // The email address of the user.
        // This is a required field and must follow a valid email format.
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        // The first name of the user.
        // This is a required field.
        [Required]
        public string FirstName { get; set; }

        // The last name of the user.
        // This is a required field.
        [Required]
        public string LastName { get; set; }

        // The phone number of the user.
        // This is a required field.
        [Required]
        public string PhoneNumber { get; set; }

        // The role of the user in the system.
        // This is a required field and uses the UserRole enum to define possible values.
        [Required]
        public UserRole Role { get; set; }

        // The date and time when the user was created.
        // Defaults to the current date and time when a new user is created.
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // The date and time of the user's last login.
        // This is optional and can be null if the user has never logged in.
        public DateTime? LastLoginDate { get; set; }

        // Indicates whether the user is active in the system.
        // Defaults to true, meaning the user is active by default.
        public bool IsActive { get; set; } = true;
    }
}