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
            var tenDaysAgo = now.AddDays(-10);
            var twelvedaysAgo = now.AddDays(-12);
            var fifteenDaysAgo = now.AddDays(-15);
            var twentyDaysAgo = now.AddDays(-20);
            var twentyFiveDaysAgo = now.AddDays(-25);
            var thirtyDaysAgo = now.AddDays(-30);
            var fortyFiveDaysAgo = now.AddDays(-45);
            var sixtyDaysAgo = now.AddDays(-60);

            // Create admin users with hashed passwords
            var adminUser1 = new User
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
            adminUser1.PasswordHash = hasher.HashPassword(adminUser1, "password");

            var adminUser2 = new User
            {
                Username = "cfrankland",
                PasswordHash = "", // Will be set after creation
                Email = "clive.frankland@agrienergyconnect.com",
                FirstName = "Clive",
                LastName = "Frankland",
                PhoneNumber = "0835559876",
                Role = UserRole.Employee,
                CreatedDate = thirtyDaysAgo,
                IsActive = true
            };
            adminUser2.PasswordHash = hasher.HashPassword(adminUser2, "password");

            // Create farmer users with hashed passwords
            var farmerUser1 = new User
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
            farmerUser1.PasswordHash = hasher.HashPassword(farmerUser1, "password");

            var farmerUser2 = new User
            {
                Username = "scooper",
                PasswordHash = "", // Will be set after creation
                Email = "sarah@sunvalleyfarm.co.za",
                FirstName = "Sarah",
                LastName = "Cooper",
                PhoneNumber = "0723456789",
                Role = UserRole.Farmer,
                CreatedDate = fifteenDaysAgo,
                IsActive = true
            };
            farmerUser2.PasswordHash = hasher.HashPassword(farmerUser2, "password");

            var farmerUser3 = new User
            {
                Username = "tmkhize",
                PasswordHash = "", // Will be set after creation
                Email = "thabo@mkhizefarms.com",
                FirstName = "Thabo",
                LastName = "Mkhize",
                PhoneNumber = "0815553421",
                Role = UserRole.Farmer,
                CreatedDate = twentyDaysAgo,
                IsActive = true
            };
            farmerUser3.PasswordHash = hasher.HashPassword(farmerUser3, "password");

            var farmerUser4 = new User
            {
                Username = "avanwyk",
                PasswordHash = "", // Will be set after creation
                Email = "anita@hilltoporganics.co.za",
                FirstName = "Anita",
                LastName = "van Wyk",
                PhoneNumber = "0664442135",
                Role = UserRole.Farmer,
                CreatedDate = thirtyDaysAgo,
                IsActive = true
            };
            farmerUser4.PasswordHash = hasher.HashPassword(farmerUser4, "password");

            var farmerUser5 = new User
            {
                Username = "dpretorius",
                PasswordHash = "", // Will be set after creation
                Email = "daniel@pretoriusfamily.co.za",
                FirstName = "Daniel",
                LastName = "Pretorius",
                PhoneNumber = "0782223344",
                Role = UserRole.Farmer,
                CreatedDate = fortyFiveDaysAgo,
                IsActive = true
            };
            farmerUser5.PasswordHash = hasher.HashPassword(farmerUser5, "password");

            var farmerUser6 = new User
            {
                Username = "lnaidoo",
                PasswordHash = "", // Will be set after creation
                Email = "leela@coastalfarms.com",
                FirstName = "Leela",
                LastName = "Naidoo",
                PhoneNumber = "0836667890",
                Role = UserRole.Farmer,
                CreatedDate = sixtyDaysAgo,
                IsActive = false // Inactive user for testing
            };
            farmerUser6.PasswordHash = hasher.HashPassword(farmerUser6, "password");

            // Add users to context
            context.Users.AddRange(adminUser1, adminUser2, farmerUser1, farmerUser2, farmerUser3, farmerUser4, farmerUser5, farmerUser6);
            await context.SaveChangesAsync();

            // Create farmer records
            var farmer1 = new Farmer
            {
                FarmName = "Green Acres Farm",
                Location = "Western Cape",
                UserId = farmerUser1.UserId
            };

            var farmer2 = new Farmer
            {
                FarmName = "Sun Valley Organic Farm",
                Location = "Eastern Cape",
                UserId = farmerUser2.UserId
            };

            var farmer3 = new Farmer
            {
                FarmName = "Mkhize Family Farms",
                Location = "KwaZulu-Natal",
                UserId = farmerUser3.UserId
            };

            var farmer4 = new Farmer
            {
                FarmName = "Hilltop Organics",
                Location = "Mpumalanga",
                UserId = farmerUser4.UserId
            };

            var farmer5 = new Farmer
            {
                FarmName = "Pretorius Family Vineyards",
                Location = "Western Cape",
                UserId = farmerUser5.UserId
            };

            var farmer6 = new Farmer
            {
                FarmName = "Coastal Farms",
                Location = "KwaZulu-Natal",
                UserId = farmerUser6.UserId
            };

            context.Farmers.AddRange(farmer1, farmer2, farmer3, farmer4, farmer5, farmer6);
            await context.SaveChangesAsync();

            // Create products for Farmer 1 (John Smith)
            var farmer1Products = new List<Product>
            {
                new Product
                {
                    Name = "Organic Maize",
                    Category = "Grains",
                    Description = "Non-GMO certified",
                    ProductionDate = fiveDaysAgo,
                    FarmerId = farmer1.FarmerId,
                    CreatedDate = fiveDaysAgo,
                    IsActive = true
                },
                new Product
                {
                    Name = "Free-range Eggs",
                    Category = "Dairy",
                    Description = "Large, brown eggs",
                    ProductionDate = eightDaysAgo,
                    FarmerId = farmer1.FarmerId,
                    CreatedDate = eightDaysAgo,
                    IsActive = true
                },
                new Product
                {
                    Name = "Fresh Milk",
                    Category = "Dairy",
                    Description = "Unpasteurised",
                    ProductionDate = sevenDaysAgo,
                    FarmerId = farmer1.FarmerId,
                    CreatedDate = sevenDaysAgo,
                    IsActive = true
                },
                new Product
                {
                    Name = "Carrots",
                    Category = "Vegetables",
                    Description = "Organic, freshly harvested",
                    ProductionDate = fifteenDaysAgo,
                    FarmerId = farmer1.FarmerId,
                    CreatedDate = fifteenDaysAgo,
                    IsActive = true
                },
                new Product
                {
                    Name = "Sweet Potatoes",
                    Category = "Vegetables",
                    Description = "Locally grown sweet potatoes",
                    ProductionDate = tenDaysAgo,
                    FarmerId = farmer1.FarmerId,
                    CreatedDate = tenDaysAgo,
                    IsActive = false
                }
            };

            // Create products for Farmer 2 (Sarah Cooper)
            var farmer2Products = new List<Product>
            {
                new Product
                {
                    Name = "Heirloom Tomatoes",
                    Category = "Vegetables",
                    Description = "Multiple varieties of colorful tomatoes",
                    ProductionDate = sevenDaysAgo,
                    FarmerId = farmer2.FarmerId,
                    CreatedDate = sevenDaysAgo,
                    IsActive = true
                },
                new Product
                {
                    Name = "Spinach",
                    Category = "Vegetables",
                    Description = "Fresh organic spinach",
                    ProductionDate = fiveDaysAgo,
                    FarmerId = farmer2.FarmerId,
                    CreatedDate = fiveDaysAgo,
                    IsActive = true
                },
                new Product
                {
                    Name = "Basil",
                    Category = "Herbs",
                    Description = "Aromatic fresh basil",
                    ProductionDate = tenDaysAgo,
                    FarmerId = farmer2.FarmerId,
                    CreatedDate = tenDaysAgo,
                    IsActive = true
                },
                new Product
                {
                    Name = "Strawberries",
                    Category = "Fruits",
                    Description = "Sweet, ripe strawberries",
                    ProductionDate = twelvedaysAgo,
                    FarmerId = farmer2.FarmerId,
                    CreatedDate = twelvedaysAgo,
                    IsActive = true
                }
            };

            // Create products for Farmer 3 (Thabo Mkhize)
            var farmer3Products = new List<Product>
            {
                new Product
                {
                    Name = "Beans",
                    Category = "Vegetables",
                    Description = "Fresh green beans",
                    ProductionDate = fiveDaysAgo,
                    FarmerId = farmer3.FarmerId,
                    CreatedDate = fiveDaysAgo,
                    IsActive = true
                },
                new Product
                {
                    Name = "Avocados",
                    Category = "Fruits",
                    Description = "Creamy Hass avocados",
                    ProductionDate = eightDaysAgo,
                    FarmerId = farmer3.FarmerId,
                    CreatedDate = eightDaysAgo,
                    IsActive = true
                },
                new Product
                {
                    Name = "Macadamia Nuts",
                    Category = "Nuts",
                    Description = "Locally grown macadamias",
                    ProductionDate = fifteenDaysAgo,
                    FarmerId = farmer3.FarmerId,
                    CreatedDate = fifteenDaysAgo,
                    IsActive = true
                }
            };

            // Create products for Farmer 4 (Anita van Wyk)
            var farmer4Products = new List<Product>
            {
                new Product
                {
                    Name = "Honey",
                    Category = "Honey",
                    Description = "Pure wildflower honey",
                    ProductionDate = tenDaysAgo,
                    FarmerId = farmer4.FarmerId,
                    CreatedDate = tenDaysAgo,
                    IsActive = true
                },
                new Product
                {
                    Name = "Lavender",
                    Category = "Herbs",
                    Description = "Fresh cut lavender bunches",
                    ProductionDate = twelvedaysAgo,
                    FarmerId = farmer4.FarmerId,
                    CreatedDate = twelvedaysAgo,
                    IsActive = true
                },
                new Product
                {
                    Name = "Sunflower Oil",
                    Category = "Organic",
                    Description = "Cold-pressed organic sunflower oil",
                    ProductionDate = twentyDaysAgo,
                    FarmerId = farmer4.FarmerId,
                    CreatedDate = twentyDaysAgo,
                    IsActive = true
                },
                new Product
                {
                    Name = "Organic Fertilizer",
                    Category = "Fertilisers",
                    Description = "Natural composted fertilizer",
                    ProductionDate = twentyFiveDaysAgo,
                    FarmerId = farmer4.FarmerId,
                    CreatedDate = twentyFiveDaysAgo,
                    IsActive = false
                }
            };

            // Create products for Farmer 5 (Daniel Pretorius)
            var farmer5Products = new List<Product>
            {
                new Product
                {
                    Name = "Cabernet Sauvignon",
                    Category = "Beverages",
                    Description = "Premium red wine from estate vineyards",
                    ProductionDate = thirtyDaysAgo,
                    FarmerId = farmer5.FarmerId,
                    CreatedDate = thirtyDaysAgo,
                    IsActive = true
                },
                new Product
                {
                    Name = "Chardonnay",
                    Category = "Beverages",
                    Description = "Barrel-aged white wine",
                    ProductionDate = thirtyDaysAgo,
                    FarmerId = farmer5.FarmerId,
                    CreatedDate = thirtyDaysAgo,
                    IsActive = true
                },
                new Product
                {
                    Name = "Table Grapes",
                    Category = "Fruits",
                    Description = "Fresh seedless grapes",
                    ProductionDate = twentyDaysAgo,
                    FarmerId = farmer5.FarmerId,
                    CreatedDate = twentyDaysAgo,
                    IsActive = true
                },
                new Product
                {
                    Name = "Grape Juice",
                    Category = "Beverages",
                    Description = "Fresh pressed grape juice",
                    ProductionDate = fifteenDaysAgo,
                    FarmerId = farmer5.FarmerId,
                    CreatedDate = fifteenDaysAgo,
                    IsActive = true
                },
                new Product
                {
                    Name = "Grape Leaves",
                    Category = "Organic",
                    Description = "Fresh grape leaves for culinary use",
                    ProductionDate = twentyDaysAgo,
                    FarmerId = farmer5.FarmerId,
                    CreatedDate = twentyDaysAgo,
                    IsActive = false
                }
            };

            // Add products to context
            context.Products.AddRange(farmer1Products);
            context.Products.AddRange(farmer2Products);
            context.Products.AddRange(farmer3Products);
            context.Products.AddRange(farmer4Products);
            context.Products.AddRange(farmer5Products);
            await context.SaveChangesAsync();

            // Create messages
            var messages = new List<Message>
            {
                new Message
                {
                    Subject = "Welcome to Agri-Energy Connect",
                    Content = "Welcome to the platform. We're excited to have you join our community of farmers and green energy experts.",
                    SenderId = adminUser1.UserId,
                    RecipientId = farmerUser1.UserId,
                    SentDate = thirtyDaysAgo,
                    ReadDate = thirtyDaysAgo // Mark as read
                },
                new Message
                {
                    Subject = "Important: New Green Energy Subsidies Available",
                    Content = "We wanted to inform you of new government subsidies for green energy solutions on farms. Contact us for more details.",
                    SenderId = adminUser1.UserId,
                    RecipientId = farmerUser1.UserId,
                    SentDate = now,
                    ReadDate = null // Unread
                },
                new Message
                {
                    Subject = "Welcome to the platform",
                    Content = "Hi Sarah, welcome to Agri-Energy Connect. We're glad to have Sun Valley Organic Farm on board. Please let us know if you have any questions.",
                    SenderId = adminUser2.UserId,
                    RecipientId = farmerUser2.UserId,
                    SentDate = fifteenDaysAgo,
                    ReadDate = fifteenDaysAgo
                },
                new Message
                {
                    Subject = "Solar panel installation query",
                    Content = "Hello, I'm interested in installing solar panels on my farm buildings. Could you provide information about costs and available subsidies?",
                    SenderId = farmerUser3.UserId,
                    RecipientId = adminUser1.UserId,
                    SentDate = tenDaysAgo,
                    ReadDate = tenDaysAgo
                },
                new Message
                {
                    Subject = "RE: Solar panel installation query",
                    Content = "Hi Thabo, thank you for your interest in solar energy solutions. We'll be scheduling a call to discuss your specific requirements and provide cost estimates. In the meantime, you can check our resources section for general information.",
                    SenderId = adminUser1.UserId,
                    RecipientId = farmerUser3.UserId,
                    SentDate = eightDaysAgo,
                    ReadDate = sevenDaysAgo
                },
                new Message
                {
                    Subject = "Biogas opportunities",
                    Content = "I've been researching biogas production and believe my farm has great potential. When would be a good time to discuss potential systems and requirements?",
                    SenderId = farmerUser4.UserId,
                    RecipientId = adminUser2.UserId,
                    SentDate = fiveDaysAgo,
                    ReadDate = null // Unread
                },
                new Message
                {
                    Subject = "Product verification",
                    Content = "Hello Clive, could you confirm that my latest products have been correctly listed on the platform? I'm particularly concerned about the organic certification display.",
                    SenderId = farmerUser5.UserId,
                    RecipientId = adminUser2.UserId,
                    SentDate = sevenDaysAgo,
                    ReadDate = sevenDaysAgo
                },
                new Message
                {
                    Subject = "RE: Product verification",
                    Content = "Hi Daniel, I've checked your product listings and all organic certifications are correctly displayed. Your Cabernet Sauvignon and Chardonnay are both showing the proper certification badges. Let me know if you need anything else.",
                    SenderId = adminUser2.UserId,
                    RecipientId = farmerUser5.UserId,
                    SentDate = fiveDaysAgo,
                    ReadDate = null // Unread
                }
            };
            context.Messages.AddRange(messages);
            await context.SaveChangesAsync();

            // Create registration requests
            var registrationRequests = new List<RegistrationRequest>
            {
                new RegistrationRequest
                {
                    Username = "dmadonsela",
                    PasswordHash = hasher.HashPassword(new User(), "password"),
                    FirstName = "David",
                    LastName = "Madonsela",
                    Email = "david@mfarms.co.za",
                    PhoneNumber = "0761234567",
                    FarmName = "Madonsela Farms",
                    Location = "Limpopo",
                    RequestDate = now.AddDays(-1),
                    Status = "Pending"
                },
                new RegistrationRequest
                {
                    Username = "vbruwer",
                    PasswordHash = hasher.HashPassword(new User(), "password"),
                    FirstName = "Vanessa",
                    LastName = "Bruwer",
                    Email = "vanessa@littlecreekfarm.co.za",
                    PhoneNumber = "0832221234",
                    FarmName = "Little Creek Farm",
                    Location = "Free State",
                    RequestDate = now.AddDays(-3),
                    Status = "Pending"
                }
            };
            context.RegistrationRequests.AddRange(registrationRequests);
            await context.SaveChangesAsync();
        }
    }
}