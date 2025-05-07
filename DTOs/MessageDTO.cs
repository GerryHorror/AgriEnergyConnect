namespace AgriEnergyConnect.DTOs
{
    // The MessageDTO class is a Data Transfer Object (DTO) used to transfer detailed message information
    // between the application layers. It includes message content, sender and recipient details, and metadata.
    public class MessageDTO
    {
        // The unique identifier for the message.
        public int MessageId { get; set; }

        // The subject or title of the message.
        public string Subject { get; set; }

        // The content or body of the message.
        public string Content { get; set; }

        // Sender information

        // The unique identifier of the user who sent the message.
        public int SenderId { get; set; }

        // The name of the user who sent the message (e.g., "John Smith").
        public string SenderName { get; set; }

        // The role of the sender in the system (e.g., "Farmer", "Admin").
        public string SenderRole { get; set; }

        // Recipient information

        // The unique identifier of the user who received the message.
        public int RecipientId { get; set; }

        // The name of the user who received the message (e.g., "Emily Mathews").
        public string RecipientName { get; set; }

        // The role of the recipient in the system (e.g., "Farmer", "Admin").
        public string RecipientRole { get; set; }

        // Message details

        // The date and time when the message was sent.
        public DateTime SentDate { get; set; }

        // Indicates whether the message has been read by the recipient.
        public bool IsRead { get; set; }
    }

    // The MessageSummaryDTO class is a lightweight Data Transfer Object (DTO) used to transfer
    // summarised message information, typically for listing or preview purposes.
    public class MessageSummaryDTO
    {
        // The unique identifier for the message.
        public int MessageId { get; set; }

        // The subject or title of the message.
        public string Subject { get; set; }

        // The unique identifier of the user who sent the message.
        public int SenderId { get; set; }

        // The name of the user who sent the message (e.g., "John Smith").
        public string SenderName { get; set; }

        // The unique identifier of the user who received the message.
        public int RecipientId { get; set; }

        // The name of the user who received the message (e.g., "Emily Mathews").
        public string RecipientName { get; set; }

        // The date and time when the message was sent.
        public DateTime SentDate { get; set; }

        // Indicates whether the message has been read by the recipient.
        public bool IsRead { get; set; }
    }
}