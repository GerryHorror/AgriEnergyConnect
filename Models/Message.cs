using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgriEnergyConnect.Models
{
    // The Message class represents a communication between users in the AgriEnergyConnect system.
    // It contains properties that store information about the message, including its sender, recipient, and content.
    public class Message
    {
        // Primary key for the Message entity.
        // This uniquely identifies each message in the database.
        [Key]
        public int MessageId { get; set; }

        // The subject of the message.
        // This is a required field with a maximum length of 100 characters.
        [Required]
        [StringLength(100)]
        public string Subject { get; set; }

        // The content or body of the message.
        // This is a required field and can contain any length of text.
        [Required]
        public string Content { get; set; }

        // Foreign key that links the Message entity to the User entity for the sender.
        // This represents the user who sent the message.
        public int SenderId { get; set; }

        // Navigation property to the User entity for the sender.
        // This allows access to the details of the user who sent the message.
        [ForeignKey("SenderId")]
        public User Sender { get; set; }

        // Foreign key that links the Message entity to the User entity for the recipient.
        // This represents the user who received the message.
        public int RecipientId { get; set; }

        // Navigation property to the User entity for the recipient.
        // This allows access to the details of the user who received the message.
        [ForeignKey("RecipientId")]
        public User Recipient { get; set; }

        // The date and time when the message was sent.
        // Defaults to the current date and time when a new message is created.
        public DateTime SentDate { get; set; } = DateTime.Now;

        // Indicates whether the message has been read by the recipient.
        // Defaults to false, meaning the message is unread by default.
        public bool IsRead { get; set; } = false;
    }
}