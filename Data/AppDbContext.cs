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

            // Configure identity columns to handle explicit ID values during seeding
            modelBuilder.Entity<User>()
                .Property(u => u.UserId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Farmer>()
                .Property(f => f.FarmerId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Product>()
                .Property(p => p.ProductId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Message>()
                .Property(m => m.MessageId)
                .ValueGeneratedOnAdd();

            // Seed the database with initial data.
            // This ensures the database is populated with default values when it is first created.
            SeedData(modelBuilder);
        }

        // Seeds the database with initial data for Users, Farmers, Products, and Messages.
        private void SeedData(ModelBuilder modelBuilder)
        {
            // Define static DateTime values to ensure consistent seeding behaviour.
            var now = new DateTime(2024, 05, 01);
            var fiveDaysAgo = now.AddDays(-5);
            var sevenDaysAgo = now.AddDays(-7);
            var eightDaysAgo = now.AddDays(-8);
            var fifteenDaysAgo = now.AddDays(-15);
            var thirtyDaysAgo = now.AddDays(-30);

            // Add an employee/admin user and a farmer user to the database.
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
                    CreatedDate = now,
                    IsActive = true
                },
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
                    CreatedDate = now,
                    IsActive = true
                }
            );

            // Add farmer record associated with the farmer user (UserId = 2).
            // This must be added before products to satisfy the foreign key constraint.
            modelBuilder.Entity<Farmer>().HasData(
                new Farmer
                {
                    FarmerId = 1,
                    FarmName = "Green Acres Farm",
                    Location = "Western Cape",
                    UserId = 2 // This links to the jsmith user
                }
            );

            // Add sample products associated with the farmer (FarmerId = 1).
            // These products must reference the correct FarmerId that was defined above.
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    ProductId = 1,
                    Name = "Organic Maize",
                    Category = "Grains",
                    Description = "Non-GMO certified",
                    ProductionDate = fiveDaysAgo,
                    FarmerId = 1, // References the Green Acres Farm
                    CreatedDate = now
                },
                new Product
                {
                    ProductId = 2,
                    Name = "Free-range Eggs",
                    Category = "Dairy",
                    Description = "Large, brown eggs",
                    ProductionDate = eightDaysAgo,
                    FarmerId = 1, // References the Green Acres Farm
                    CreatedDate = now
                },
                new Product
                {
                    ProductId = 3,
                    Name = "Fresh Milk",
                    Category = "Dairy",
                    Description = "Unpasteurised",
                    ProductionDate = sevenDaysAgo,
                    FarmerId = 1, // References the Green Acres Farm
                    CreatedDate = now
                },
                new Product
                {
                    ProductId = 4,
                    Name = "Carrots",
                    Category = "Vegetables",
                    Description = "Organic, freshly harvested",
                    ProductionDate = fifteenDaysAgo,
                    FarmerId = 1, // References the Green Acres Farm
                    CreatedDate = now
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
                    SentDate = thirtyDaysAgo,
                    IsRead = true
                },
                new Message
                {
                    MessageId = 2,
                    Subject = "Important: New Green Energy Subsidies Available",
                    Content = "We wanted to inform you of new government subsidies for green energy solutions on farms. Contact us for more details.",
                    SenderId = 1, // Admin, Emily Mathews
                    RecipientId = 2, // Farmer, John Smith
                    SentDate = now,
                    IsRead = false
                }
            );
        }
    }
}