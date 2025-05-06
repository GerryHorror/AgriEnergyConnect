using AgriEnergyConnect.Data;
using AgriEnergyConnect.Models;
using AgriEnergyConnect.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgriEnergyConnect.Repositories.Implementation
{
    // This class implements the IMessageRepository interface and provides the actual logic
    // for interacting with the Message data in the database.
    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext _context;

        // Constructor that accepts the AppDbContext to interact with the database.
        // Dependency injection is used to provide the context.
        public MessageRepository(AppDbContext context)
        {
            _context = context;
        }

        // Fetches all messages received by a specific user (inbox).
        // Includes the sender details for each message.
        // Parameters:
        //   userId - The ID of the user whose inbox messages you want to retrieve.
        // Returns a list of Message objects representing the user's inbox, ordered by the most recent messages.
        public async Task<IEnumerable<Message>> GetInboxMessagesAsync(int userId)
        {
            return await _context.Messages
                .Include(m => m.Sender) // Includes the sender details.
                .Where(m => m.RecipientId == userId) // Filters messages by recipient ID.
                .OrderByDescending(m => m.SentDate) // Orders messages by sent date (newest first).
                .ToListAsync(); // Converts the result to a list asynchronously.
        }

        // Fetches all messages sent by a specific user.
        // Includes the recipient details for each message.
        // Parameters:
        //   userId - The ID of the user whose sent messages you want to retrieve.
        // Returns a list of Message objects representing the user's sent messages, ordered by the most recent messages.
        public async Task<IEnumerable<Message>> GetSentMessagesAsync(int userId)
        {
            return await _context.Messages
                .Include(m => m.Recipient) // Includes the recipient details.
                .Where(m => m.SenderId == userId) // Filters messages by sender ID.
                .OrderByDescending(m => m.SentDate) // Orders messages by sent date (newest first).
                .ToListAsync(); // Converts the result to a list asynchronously.
        }

        // Finds a specific message by its unique ID (MessageId).
        // Includes both the sender and recipient details.
        // Parameters:
        //   messageId - The ID of the message you’re looking for.
        // Returns the Message object if found, or null if it doesn’t exist.
        public async Task<Message> GetMessageByIdAsync(int messageId)
        {
            return await _context.Messages
                .Include(m => m.Sender) // Includes the sender details.
                .Include(m => m.Recipient) // Includes the recipient details.
                .FirstOrDefaultAsync(m => m.MessageId == messageId); // Finds the first match or returns null.
        }

        // Adds a new message to the database.
        // Automatically sets the SentDate to the current date and time and marks the message as unread.
        // Parameters:
        //   message - The Message object containing the details of the new message.
        // Saves the message to the database asynchronously.
        public async Task AddMessageAsync(Message message)
        {
            message.SentDate = DateTime.Now; // Sets the sent date to the current date and time.
            message.IsRead = false; // Marks the message as unread.

            await _context.Messages.AddAsync(message); // Adds the message to the context.
            await _context.SaveChangesAsync(); // Saves the changes to the database.
        }

        // Marks a specific message as read.
        // Updates the IsRead property of the message in the database.
        // Parameters:
        //   messageId - The ID of the message to mark as read.
        // If the message exists and is unread, it will be updated.
        public async Task MarkAsReadAsync(int messageId)
        {
            var message = await _context.Messages.FindAsync(messageId); // Finds the message by its ID.

            if (message != null && !message.IsRead) // Checks if the message exists and is unread.
            {
                message.IsRead = true; // Marks the message as read.
                await _context.SaveChangesAsync(); // Saves the changes to the database.
            }
        }

        // Checks if a message exists in the database by its unique ID (MessageId).
        // Parameters:
        //   messageId - The ID of the message you want to check.
        // Returns true if the message exists, or false if it doesn’t.
        public async Task<bool> MessageExistsAsync(int messageId)
        {
            return await _context.Messages
                .AnyAsync(m => m.MessageId == messageId); // Checks if any message matches the given ID.
        }
    }
}