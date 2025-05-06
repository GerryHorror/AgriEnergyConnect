using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgriEnergyConnect.Models
{
    // The Farmer class represents a farmer in the AgriEnergyConnect system.
    // It contains properties that store information about the farmer and their associated data.
    public class Farmer
    {
        // Primary key for the Farmer entity.
        // This uniquely identifies each farmer in the database.
        [Key]
        public int FarmerId { get; set; }

        // The name of the farm associated with the farmer.
        // This is a required field with a maximum length of 100 characters.
        [Required]
        [StringLength(100)]
        public string FarmName { get; set; }

        // The location of the farm.
        // This is a required field with a maximum length of 100 characters.
        [Required]
        [StringLength(100)]
        public string Location { get; set; }

        // Foreign key that links the Farmer entity to the User entity.
        // This establishes a relationship between a farmer and their user account.
        public int UserId { get; set; }

        // Navigation property to the User entity.
        // This allows access to the user details associated with the farmer.
        [ForeignKey("UserId")]
        public User User { get; set; }

        // Navigation property for the collection of products associated with the farmer.
        // This represents a one-to-many relationship where a farmer can have multiple products.
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}