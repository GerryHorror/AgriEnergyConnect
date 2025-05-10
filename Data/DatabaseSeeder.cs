using AgriEnergyConnect.Models;
using Microsoft.AspNetCore.Identity;

namespace AgriEnergyConnect.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedDataAsync(AppDbContext context)
        {
            // Check if data already exists
            if (context.Users.Any())
            {
                return; // Database has been seeded
            }

            // Create password hasher
            var hasher = new PasswordHasher<User>();

            // Define static DateTime values
            var now = new DateTime(2024, 05, 01);
            var fiveDaysAgo = now.AddDays(-5);
            var sevenDaysAgo = now.AddDays(-7);
            var eightDaysAgo = now.AddDays(-8);
            var fifteenDaysAgo = now.AddDays(-15);
            var thirtyDaysAgo = now.AddDays(-30);

            // Create users with hashed passwords
            var adminUser = new User
            {
                Username = "admin",
                PasswordHash = "", // Will be set after creation
                Email = "admin@agrienergyconnect.com",
                FirstName = "Emily",
                LastName = "Mathews",
                PhoneNumber = "0841258975",
                Role = UserRole.Employee,
                CreatedDate = now,
                IsActive = true
            };
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "password");

            var farmerUser = new User
            {
                Username = "jsmith",
                PasswordHash = "", // Will be set after creation
                Email = "john@greenacres.com",
                FirstName = "John",
                LastName = "Smith",
                PhoneNumber = "0601091293",
                Role = UserRole.Farmer,
                CreatedDate = now,
                IsActive = true
            };
            farmerUser.PasswordHash = hasher.HashPassword(farmerUser, "password");

            // Add users to context
            context.Users.AddRange(adminUser, farmerUser);
            await context.SaveChangesAsync();

            // Create farmer record
            var farmer = new Farmer
            {
                FarmName = "Green Acres Farm",
                Location = "Western Cape",
                UserId = farmerUser.UserId
            };
            context.Farmers.Add(farmer);
            await context.SaveChangesAsync();

            // Create products
            var products = new List<Product>
            {
                new Product
                {
                    Name = "Organic Maize",
                    Category = "Grains",
                    Description = "Non-GMO certified",
                    ProductionDate = fiveDaysAgo,
                    FarmerId = farmer.FarmerId,
                    CreatedDate = now
                },
                new Product
                {
                    Name = "Free-range Eggs",
                    Category = "Dairy",
                    Description = "Large, brown eggs",
                    ProductionDate = eightDaysAgo,
                    FarmerId = farmer.FarmerId,
                    CreatedDate = now
                },
                new Product
                {
                    Name = "Fresh Milk",
                    Category = "Dairy",
                    Description = "Unpasteurised",
                    ProductionDate = sevenDaysAgo,
                    FarmerId = farmer.FarmerId,
                    CreatedDate = now
                },
                new Product
                {
                    Name = "Carrots",
                    Category = "Vegetables",
                    Description = "Organic, freshly harvested",
                    ProductionDate = fifteenDaysAgo,
                    FarmerId = farmer.FarmerId,
                    CreatedDate = now
                }
            };
            context.Products.AddRange(products);
            await context.SaveChangesAsync();

            // Create messages
            var messages = new List<Message>
            {
                new Message
                {
                    Subject = "Welcome to Agri-Energy Connect",
                    Content = "Welcome to the platform. We're excited to have you join our community of farmers and green energy experts.",
                    SenderId = adminUser.UserId,
                    RecipientId = farmerUser.UserId,
                    SentDate = thirtyDaysAgo,
                    IsRead = true
                },
                new Message
                {
                    Subject = "Important: New Green Energy Subsidies Available",
                    Content = "We wanted to inform you of new government subsidies for green energy solutions on farms. Contact us for more details.",
                    SenderId = adminUser.UserId,
                    RecipientId = farmerUser.UserId,
                    SentDate = now,
                    IsRead = false
                }
            };
            context.Messages.AddRange(messages);
            await context.SaveChangesAsync();
        }
    }
}