using AgriEnergyConnect.Models;

namespace AgriEnergyConnect.Repositories.Interfaces
{
    // This interface defines the methods for working with Message data.
    // It’s like a contract that ensures any class implementing it will provide these functionalities.
    public interface IMessageRepository
    {
        // Fetches all messages received by a specific user (inbox).
        // Parameters:
        //   userId - The ID of the user whose inbox messages you want to retrieve.
        // Returns a list of Message objects representing the user's inbox.
        Task<IEnumerable<Message>> GetInboxMessagesAsync(int userId);

        // Fetches all messages sent by a specific user.
        // Parameters:
        //   userId - The ID of the user whose sent messages you want to retrieve.
        // Returns a list of Message objects representing the user's sent messages.
        Task<IEnumerable<Message>> GetSentMessagesAsync(int userId);

        // Finds a specific message by its unique ID (MessageId).
        // Parameters:
        //   messageId - The ID of the message you’re looking for.
        // Returns the Message object if found, or null if it doesn’t exist.
        Task<Message> GetMessageByIdAsync(int messageId);

        // Adds a new message to the database.
        // Parameters:
        //   message - The Message object containing the details of the new message.
        // Saves the message to the database asynchronously.
        Task AddMessageAsync(Message message);

        // Marks a specific message as read.
        // Parameters:
        //   messageId - The ID of the message to mark as read.
        // Updates the IsRead property of the message in the database.
        Task MarkAsReadAsync(int messageId);

        // Checks if a message exists in the database by its unique ID (MessageId).
        // Parameters:
        //   messageId - The ID of the message you want to check.
        // Returns true if the message exists, or false if it doesn’t.
        Task<bool> MessageExistsAsync(int messageId);
    }
}