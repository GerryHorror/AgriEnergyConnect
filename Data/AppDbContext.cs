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
        }
    }
}