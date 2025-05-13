using AgriEnergyConnect.DTOs;
using AgriEnergyConnect.Models;
using AgriEnergyConnect.Repositories.Interfaces;
using AgriEnergyConnect.Services.Interfaces;
using AgriEnergyConnect.Helpers;
using AgriEnergyConnect.Data;
using Microsoft.EntityFrameworkCore;

namespace AgriEnergyConnect.Services.Implementation
{
    // The MessageService class implements the IMessageService interface and provides the business logic
    // for managing messages in the system. It interacts with the repository layer to perform CRUD operations
    // and applies additional mapping and validation logic as needed.
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly AppDbContext _context;

        // Constructor that accepts the IMessageRepository to interact with the database.
        // Dependency injection is used to provide the repository.
        public MessageService(IMessageRepository messageRepository, AppDbContext context)
        {
            _messageRepository = messageRepository;
            _context = context;
        }

        // Retrieves all messages received by a specific user (inbox).
        // Parameters:
        //   userId - The ID of the user whose inbox messages you want to retrieve.
        // Returns a collection of Message objects representing the user's inbox.
        public async Task<IEnumerable<Message>> GetInboxMessagesAsync(int userId)
        {
            return await _messageRepository.GetInboxMessagesAsync(userId);
        }

        // Retrieves summarised information for all messages received by a specific user (inbox).
        // Parameters:
        //   userId - The ID of the user whose inbox message summaries you want to retrieve.
        // Returns a collection of MessageSummaryDTO objects for the user's inbox.
        public async Task<IEnumerable<MessageSummaryDTO>> GetInboxMessagesSummariesAsync(int userId)
        {
            var messages = await _messageRepository.GetInboxMessagesAsync(userId);
            return MappingHelper.ToMessageSummaryDTOList(messages);
        }

        // Retrieves all messages sent by a specific user.
        // Parameters:
        //   userId - The ID of the user whose sent messages you want to retrieve.
        // Returns a collection of Message objects representing the user's sent messages.
        public async Task<IEnumerable<Message>> GetSentMessagesAsync(int userId)
        {
            return await _messageRepository.GetSentMessagesAsync(userId);
        }

        // Retrieves summarised information for all messages sent by a specific user.
        // Parameters:
        //   userId - The ID of the user whose sent message summaries you want to retrieve.
        // Returns a collection of MessageSummaryDTO objects for the user's sent messages.
        public async Task<IEnumerable<MessageSummaryDTO>> GetSentMessagesSummariesAsync(int userId)
        {
            var messages = await _messageRepository.GetSentMessagesAsync(userId);
            return MappingHelper.ToMessageSummaryDTOList(messages);
        }

        // Retrieves a specific message by its unique ID (MessageId).
        // Parameters:
        //   messageId - The ID of the message you're looking for.
        // Returns the Message object if found, or null if it doesn't exist.
        public async Task<Message> GetMessageByIdAsync(int messageId)
        {
            return await _messageRepository.GetMessageByIdAsync(messageId);
        }

        // Retrieves detailed information for a specific message by its unique ID (MessageId).
        // Parameters:
        //   messageId - The ID of the message you're looking for.
        // Returns a MessageDTO containing detailed information about the message, or null if it doesn't exist.
        public async Task<MessageDTO> GetMessageDTOByIdAsync(int messageId)
        {
            var message = await _messageRepository.GetMessageByIdAsync(messageId);
            return MappingHelper.ToMessageDTO(message);
        }

        // Sends a new message from one user to another.
        // Parameters:
        //   senderId - The ID of the user sending the message.
        //   recipientId - The ID of the user receiving the message.
        //   subject - The subject of the message.
        //   content - The content or body of the message.
        // Returns the newly created Message object.
        // Throws an ArgumentException if the subject or content is null or empty.
        public async Task<Message> SendMessageAsync(int senderId, int recipientId, string subject, string content)
        {
            if (string.IsNullOrEmpty(subject))
            {
                throw new ArgumentException("Subject is required.");
            }
            if (string.IsNullOrEmpty(content))
            {
                throw new ArgumentException("Content is required.");
            }

            var message = new Message
            {
                SenderId = senderId,
                RecipientId = recipientId,
                Subject = subject,
                Content = content,
                SentDate = DateTime.Now
            };

            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
            return message;
        }

        // Retrieves a conversation between two users.
        // Parameters:
        //   userId1 - The ID of the first user in the conversation.
        //   userId2 - The ID of the second user in the conversation.
        // Returns a collection of Message objects representing the conversation.
        public async Task<IEnumerable<Message>> GetConversationAsync(int userId1, int userId2)
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Recipient)
                .Where(m => (m.SenderId == userId1 && m.RecipientId == userId2) ||
                           (m.SenderId == userId2 && m.RecipientId == userId1))
                .OrderBy(m => m.SentDate)
                .ToListAsync();
        }

        // Retrieves unread messages for a specific user.
        // Parameters:
        //   userId - The ID of the user whose unread messages you want to retrieve.
        // Returns a collection of Message objects representing the user's unread messages.
        public async Task<IEnumerable<Message>> GetUnreadMessagesAsync(int userId)
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .Where(m => m.RecipientId == userId && m.ReadDate == null)
                .OrderByDescending(m => m.SentDate)
                .ToListAsync();
        }

        // Retrieves the count of unread messages for a specific user.
        // Parameters:
        //   userId - The ID of the user whose unread message count you want to retrieve.
        // Returns the count of unread messages for the user.
        public async Task<int> GetUnreadMessageCountAsync(int userId)
        {
            return await _context.Messages
                .CountAsync(m => m.RecipientId == userId && m.ReadDate == null);
        }

        // Marks a specific message as read by a user.
        // Parameters:
        //   messageId - The ID of the message to mark as read.
        //   userId - The ID of the user marking the message as read.
        // Returns true if the operation was successful, or false if the message doesn't exist or the user is not authorized.
        public async Task<bool> MarkAsReadAsync(int messageId, int userId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message != null && message.RecipientId == userId)
            {
                message.ReadDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // Validates message input parameters
        // Parameters:
        //   subject - The subject of the message
        //   content - The content of the message
        //   recipientId - The ID of the recipient
        // Returns true if all parameters are valid, false otherwise
        public bool ValidateMessageInput(string subject, string content, int recipientId)
        {
            return !string.IsNullOrWhiteSpace(subject) &&
                   !string.IsNullOrWhiteSpace(content) &&
                   recipientId != 0;
        }

        // Checks if a user has access to a message
        // Parameters:
        //   messageId - The ID of the message
        //   userId - The ID of the user
        // Returns true if the user has access to the message, false otherwise
        public async Task<bool> HasMessageAccessAsync(int messageId, int userId)
        {
            var message = await _messageRepository.GetMessageByIdAsync(messageId);
            return message != null && (message.RecipientId == userId || message.SenderId == userId);
        }
    }
}