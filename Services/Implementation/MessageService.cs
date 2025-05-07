using AgriEnergyConnect.DTOs;
using AgriEnergyConnect.Models;
using AgriEnergyConnect.Repositories.Interfaces;
using AgriEnergyConnect.Services.Interfaces;
using AgriEnergyConnect.Helpers;

namespace AgriEnergyConnect.Services.Implementation
{
    // The MessageService class implements the IMessageService interface and provides the business logic
    // for managing messages in the system. It interacts with the repository layer to perform CRUD operations
    // and applies additional mapping and validation logic as needed.
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;

        // Constructor that accepts the IMessageRepository to interact with the database.
        // Dependency injection is used to provide the repository.
        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
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
        //   messageId - The ID of the message you’re looking for.
        // Returns the Message object if found, or null if it doesn’t exist.
        public async Task<Message> GetMessageByIdAsync(int messageId)
        {
            return await _messageRepository.GetMessageByIdAsync(messageId);
        }

        // Retrieves detailed information for a specific message by its unique ID (MessageId).
        // Parameters:
        //   messageId - The ID of the message you’re looking for.
        // Returns a MessageDTO containing detailed information about the message, or null if it doesn’t exist.
        public async Task<MessageDTO> GetMessageDTOByIdAsync(int messageId)
        {
            var message = await _messageRepository.GetMessageByIdAsync(messageId);
            return MappingHelper.ToMessageDTO(message);
        }

        // Sends a new message from one user to another.
        // Parameters:
        //   senderId - The ID of the user sending the message.
        //   recipientId - The ID of the user receiving the message.
        //   subject - The subject or title of the message.
        //   content - The content or body of the message.
        // Returns the newly created Message object.
        // Throws an ArgumentException if the subject or content is null or empty.
        public async Task<Message> SendMessageAsync(int senderId, int recipientId, string subject, string content)
        {
            if (string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(content))
            {
                throw new ArgumentException("Subject and content are required.");
            }

            var message = new Message
            {
                Subject = subject,
                Content = content,
                SenderId = senderId,
                RecipientId = recipientId,
                SentDate = DateTime.Now,
                IsRead = false
            };

            await _messageRepository.AddMessageAsync(message);
            return message;
        }

        // Marks a specific message as read by a user.
        // Parameters:
        //   messageId - The ID of the message to mark as read.
        //   userId - The ID of the user marking the message as read.
        // Returns true if the operation was successful, or false if the message doesn’t exist
        // or the user is not authorised to mark it as read.
        public async Task<bool> MarkAsReadAsync(int messageId, int userId)
        {
            var message = await _messageRepository.GetMessageByIdAsync(messageId);
            if (message == null)
                return false;

            // Ensure only the recipient can mark the message as read.
            if (message.RecipientId != userId)
                return false;

            if (!message.IsRead)
            {
                message.IsRead = true;
                await _messageRepository.MarkAsReadAsync(messageId);
                return true;
            }

            return false; // Message already marked as read.
        }
    }
}