using AgriEnergyConnect.Models;
using Microsoft.EntityFrameworkCore;

namespace AgriEnergyConnect.Data
{
    // The AppDbContext class represents the database context for the AgriEnergyConnect system.
    // It is responsible for managing the database connections and providing access to the application's entities.
    public class AppDbContext : DbContext
    {
        // Constructor that accepts DbContextOptions to configure the context.
        // This allows dependency injection to provide the necessary configuration.
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSet representing the Users table in the database.
        // This provides access to the User entities.
        public DbSet<User> Users { get; set; }

        // DbSet representing the Farmers table in the database.
        // This provides access to the Farmer entities.
        public DbSet<Farmer> Farmers { get; set; }

        // DbSet representing the Products table in the database.
        // This provides access to the Product entities.
        public DbSet<Product> Products { get; set; }

        // DbSet representing the Messages table in the database.
        // This provides access to the Message entities.
        public DbSet<Message> Messages { get; set; }

        // Configures the model and relationships between entities.
        // This method is called when the model is being created.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the relationship between Message and User for the Sender.
            // A message has one sender, and deleting a user will not delete their sent messages.
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the relationship between Message and User for the Recipient.
            // A message has one recipient, and deleting a user will not delete their received messages.
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Recipient)
                .WithMany()
                .HasForeignKey(m => m.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed the database with initial data.
            // This ensures the database is populated with default values when it is first created.
            SeedData(modelBuilder);
        }

        // Seeds the database with initial data for Users, Products, and Messages.
        private void SeedData(ModelBuilder modelBuilder)
        {
            // Add an employee/admin user to the database.
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    Username = "admin",
                    // In a real application, the password would be securely hashed.
                    PasswordHash = "password",
                    Email = "admin@agrienergyconnect.com",
                    FirstName = "Emily",
                    LastName = "Mathews",
                    PhoneNumber = "0841258975",
                    Role = UserRole.Employee,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                }
            );

            // Add a sample farmer user to the database.
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 2,
                    Username = "jsmith",
                    // In a real application, the password would be securely hashed.
                    PasswordHash = "password",
                    Email = "john@greenacres.com",
                    FirstName = "John",
                    LastName = "Smith",
                    PhoneNumber = "0601091293",
                    Role = UserRole.Farmer,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                }
            );

            // Add sample products associated with the farmer.
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    ProductId = 1,
                    Name = "Organic Maize",
                    Category = "Grains",
                    Description = "Non-GMO certified",
                    ProductionDate = DateTime.Now.AddDays(-5),
                    FarmerId = 1,
                    CreatedDate = DateTime.Now
                },
                new Product
                {
                    ProductId = 2,
                    Name = "Free-range Eggs",
                    Category = "Dairy",
                    Description = "Large, brown eggs",
                    ProductionDate = DateTime.Now.AddDays(-8),
                    FarmerId = 1,
                    CreatedDate = DateTime.Now
                },
                new Product
                {
                    ProductId = 3,
                    Name = "Fresh Milk",
                    Category = "Dairy",
                    Description = "Unpasteurised",
                    ProductionDate = DateTime.Now.AddDays(-7),
                    FarmerId = 1,
                    CreatedDate = DateTime.Now
                },
                new Product
                {
                    ProductId = 4,
                    Name = "Carrots",
                    Category = "Vegetables",
                    Description = "Organic, freshly harvested",
                    ProductionDate = DateTime.Now.AddDays(-15),
                    FarmerId = 1,
                    CreatedDate = DateTime.Now
                }
            );

            // Add sample messages between the admin and the farmer.
            modelBuilder.Entity<Message>().HasData(
                new Message
                {
                    MessageId = 1,
                    Subject = "Welcome to Agri-Energy Connect",
                    Content = "Welcome to the platform. We're excited to have you join our community of farmers and green energy experts.",
                    SenderId = 1, // Admin, Emily Mathews
                    RecipientId = 2, // Farmer, John Smith
                    SentDate = DateTime.Now.AddDays(-30),
                    IsRead = true
                },
                new Message
                {
                    MessageId = 2,
                    Subject = "Important: New Green Energy Subsidies Available",
                    Content = "We wanted to inform you of new government subsidies for green energy solutions on farms. Contact us for more details.",
                    SenderId = 1, // Admin, Emily Mathews
                    RecipientId = 2, // Farmer, John Smith
                    SentDate = DateTime.Now,
                    IsRead = false
                }
            );
        }
    }
}