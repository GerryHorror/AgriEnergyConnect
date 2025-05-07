using AgriEnergyConnect.DTOs;
using AgriEnergyConnect.Models;

namespace AgriEnergyConnect.Services.Interfaces
{
    // The IMessageService interface defines the contract for managing messages in the system.
    // It provides methods for retrieving, sending, and updating messages, ensuring a clean separation of concerns.
    public interface IMessageService
    {
        // Retrieves all messages received by a specific user (inbox).
        // Parameters:
        //   userId - The ID of the user whose inbox messages you want to retrieve.
        // Returns a collection of Message objects representing the user's inbox.
        Task<IEnumerable<Message>> GetInboxMessagesASync(int userId);

        // Retrieves summarised information for all messages received by a specific user (inbox).
        // Parameters:
        //   userId - The ID of the user whose inbox message summaries you want to retrieve.
        // Returns a collection of MessageSummaryDTO objects for the user's inbox.
        Task<IEnumerable<MessageSummaryDTO>> GetInboxMessagesSummariesASync(int userId);

        // Retrieves all messages sent by a specific user.
        // Parameters:
        //   userId - The ID of the user whose sent messages you want to retrieve.
        // Returns a collection of Message objects representing the user's sent messages.
        Task<IEnumerable<Message>> GetSentMessagesASync(int userId);

        // Retrieves summarised information for all messages sent by a specific user.
        // Parameters:
        //   userId - The ID of the user whose sent message summaries you want to retrieve.
        // Returns a collection of MessageSummaryDTO objects for the user's sent messages.
        Task<IEnumerable<MessageSummaryDTO>> GetSentMessagesSummariesASync(int userId);

        // Retrieves a specific message by its unique ID (MessageId).
        // Parameters:
        //   messageId - The ID of the message you’re looking for.
        // Returns the Message object if found, or null if it doesn’t exist.
        Task<Message> GetMessageByIdAsync(int messageId);

        // Retrieves detailed information for a specific message by its unique ID (MessageId).
        // Parameters:
        //   messageId - The ID of the message you’re looking for.
        // Returns a MessageDTO containing detailed information about the message, or null if it doesn’t exist.
        Task<MessageDTO> GetMessageDTOByIdAsync(int messageId);

        // Sends a new message from one user to another.
        // Parameters:
        //   senderId - The ID of the user sending the message.
        //   recipientId - The ID of the user receiving the message.
        //   subject - The subject or title of the message.
        //   content - The content or body of the message.
        // Returns the newly created Message object.
        Task<Message> SendMessageAsync(int senderId, int recipientId, string subject, string content);

        // Marks a specific message as read by a user.
        // Parameters:
        //   messageId - The ID of the message to mark as read.
        //   userId - The ID of the user marking the message as read.
        // Returns true if the operation was successful, or false if the message doesn’t exist or the user is not authorised.
        Task<bool> MarkAsReadAsync(int messageId, int userId);
    }
}