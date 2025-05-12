using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgriEnergyConnect.Models
{
    // The Product class represents a product in the AgriEnergyConnect system.
    // It contains properties that store information about the product and its relationship to a farmer.
    public class Product
    {
        // Primary key for the Product entity.
        // This uniquely identifies each product in the database.
        [Key]
        public int ProductId { get; set; }

        // The name of the product.
        // This is a required field with a maximum length of 100 characters.
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        // The category of the product (e.g., fruits, vegetables, grains).
        // This is a required field with a maximum length of 100 characters.
        [Required]
        [StringLength(100)]
        public string Category { get; set; }

        // The date when the product was produced.
        // This is a required field and must be a valid date.
        [Required]
        [DataType(DataType.Date)]
        public DateTime ProductionDate { get; set; }

        // A brief description of the product.
        // This is an optional field with a maximum length of 500 characters.
        [StringLength(500)]
        public string Description { get; set; }

        // Foreign key that links the Product entity to the Farmer entity.
        // This establishes a relationship between a product and the farmer who produced it.
        public int FarmerId { get; set; }

        // Navigation property to the Farmer entity.
        // This allows access to the farmer details associated with the product.
        [ForeignKey("FarmerId")]
        public Farmer Farmer { get; set; }

        // The date and time when the product was created in the system.
        // Defaults to the current date and time when a new product is added.
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true; // Sets the default status of the product to active.
    }
}