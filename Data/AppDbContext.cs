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

        // DbSet representing the RegistrationRequests table in the database.
        // This provides access to the RegistrationRequest entities.
        public DbSet<RegistrationRequest> RegistrationRequests { get; set; }

        // Configures the model and relationships between entities.
        // This method is called when the model is being created.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Configure Farmer entity
            modelBuilder.Entity<Farmer>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Product entity
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Farmer)
                .WithMany(f => f.Products)
                .HasForeignKey(p => p.FarmerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Message entity
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Recipient)
                .WithMany()
                .HasForeignKey(m => m.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}