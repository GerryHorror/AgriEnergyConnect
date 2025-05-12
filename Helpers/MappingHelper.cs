using AgriEnergyConnect.DTOs;
using AgriEnergyConnect.Models;
using System.Collections.Generic;
using System.Linq;

namespace AgriEnergyConnect.Helpers
{
    // The MappingHelper class provides static methods to map entities to their corresponding Data Transfer Objects (DTOs).
    // This ensures a clean separation between the data models and the data sent to or received from the client.
    public static class MappingHelper
    {
        // Farmer mapping methods

        // Maps a Farmer entity to a FarmerDTO.
        // Parameters:
        //   farmer - The Farmer entity to map.
        // Returns a FarmerDTO containing detailed farmer information, or null if the input is null.
        public static FarmerDTO ToFarmerDTO(Farmer farmer)
        {
            if (farmer == null)
                return null;

            return new FarmerDTO
            {
                FarmerId = farmer.FarmerId,
                FarmName = farmer.FarmName,
                Location = farmer.Location,
                UserId = farmer.UserId,
                Username = farmer.User?.Username,
                FirstName = farmer.User?.FirstName,
                LastName = farmer.User?.LastName,
                Email = farmer.User?.Email,
                PhoneNumber = farmer.User?.PhoneNumber,
                JoinedDate = farmer.User?.CreatedDate ?? System.DateTime.Now,
                ProductCount = farmer.Products?.Count ?? 0,
                IsActive = farmer.User?.IsActive ?? false,
                RecentProducts = farmer.Products?
                    .OrderByDescending(p => p.CreatedDate) // Orders products by creation date (newest first).
                    .Take(5) // Limits the result to the 5 most recent products.
                    .Select(p => new ProductSummaryDTO
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        Category = p.Category,
                        ProductionDate = p.ProductionDate
                    })
                    .ToList()
            };
        }

        // Maps a Farmer entity to a FarmerSummaryDTO.
        // Parameters:
        //   farmer - The Farmer entity to map.
        // Returns a FarmerSummaryDTO containing summarised farmer information, or null if the input is null.
        public static FarmerSummaryDTO ToFarmerSummaryDTO(Farmer farmer)
        {
            if (farmer == null)
                return null;

            return new FarmerSummaryDTO
            {
                FarmerId = farmer.FarmerId,
                FarmName = farmer.FarmName,
                Location = farmer.Location,
                OwnerName = $"{farmer.User?.FirstName ?? "Unknown"} {farmer.User?.LastName ?? "Farmer"}".Trim(),
                ProductCount = farmer.Products?.Count ?? 0,
                IsActive = farmer.User?.IsActive ?? false
            };
        }

        // Maps a collection of Farmer entities to a list of FarmerSummaryDTOs.
        // Parameters:
        //   farmers - The collection of Farmer entities to map.
        // Returns a list of FarmerSummaryDTOs, or an empty list if the input is null.
        public static List<FarmerSummaryDTO> ToFarmerSummaryDTOList(IEnumerable<Farmer> farmers)
        {
            if (farmers == null)
                return new List<FarmerSummaryDTO>();

            return farmers.Select(f => ToFarmerSummaryDTO(f)).ToList();
        }

        // Product mapping methods

        // Maps a Product entity to a ProductDTO.
        // Parameters:
        //   product - The Product entity to map.
        // Returns a ProductDTO containing detailed product information, or null if the input is null.
        public static ProductDTO ToProductDTO(Product product)
        {
            if (product == null)
                return null;

            return new ProductDTO
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Category = product.Category,
                ProductionDate = product.ProductionDate,
                Description = product.Description,
                FarmerId = product.FarmerId,
                FarmerName = $"{product.Farmer?.User?.FirstName} {product.Farmer?.User?.LastName}",
                FarmName = product.Farmer?.FarmName,
                CreatedDate = product.CreatedDate,
                IsActive = product.IsActive
            };
        }

        // Maps a Product entity to a ProductSummaryDTO.
        // Parameters:
        //   product - The Product entity to map.
        // Returns a ProductSummaryDTO containing summarised product information, or null if the input is null.
        public static ProductSummaryDTO ToProductSummaryDTO(Product product)
        {
            if (product == null)
                return null;

            return new ProductSummaryDTO
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Category = product.Category,
                ProductionDate = product.ProductionDate,
                IsActive = product.IsActive
            };
        }

        // Maps a collection of Product entities to a list of ProductDTOs.
        // Parameters:
        //   products - The collection of Product entities to map.
        // Returns a list of ProductDTOs, or an empty list if the input is null.
        public static List<ProductDTO> ToProductDTOList(IEnumerable<Product> products)
        {
            if (products == null)
                return new List<ProductDTO>();

            return products.Select(p => ToProductDTO(p)).ToList();
        }

        // Message mapping methods

        // Maps a Message entity to a MessageDTO.
        // Parameters:
        //   message - The Message entity to map.
        // Returns a MessageDTO containing detailed message information, or null if the input is null.
        public static MessageDTO ToMessageDTO(Message message)
        {
            if (message == null)
                return null;

            return new MessageDTO
            {
                MessageId = message.MessageId,
                Subject = message.Subject,
                Content = message.Content,
                SenderId = message.SenderId,
                SenderName = $"{message.Sender?.FirstName} {message.Sender?.LastName}",
                SenderRole = message.Sender?.Role.ToString(),
                RecipientId = message.RecipientId,
                RecipientName = $"{message.Recipient?.FirstName} {message.Recipient?.LastName}",
                RecipientRole = message.Recipient?.Role.ToString(),
                SentDate = message.SentDate,
                IsRead = message.IsRead
            };
        }

        // Maps a Message entity to a MessageSummaryDTO.
        // Parameters:
        //   message - The Message entity to map.
        // Returns a MessageSummaryDTO containing summarised message information, or null if the input is null.
        public static MessageSummaryDTO ToMessageSummaryDTO(Message message)
        {
            if (message == null)
                return null;

            return new MessageSummaryDTO
            {
                MessageId = message.MessageId,
                Subject = message.Subject,
                SenderId = message.SenderId,
                SenderName = $"{message.Sender?.FirstName} {message.Sender?.LastName}",
                RecipientId = message.RecipientId,
                RecipientName = $"{message.Recipient?.FirstName} {message.Recipient?.LastName}",
                SentDate = message.SentDate,
                IsRead = message.IsRead
            };
        }

        // Maps a collection of Message entities to a list of MessageSummaryDTOs.
        // Parameters:
        //   messages - The collection of Message entities to map.
        // Returns a list of MessageSummaryDTOs, or an empty list if the input is null.
        public static List<MessageSummaryDTO> ToMessageSummaryDTOList(IEnumerable<Message> messages)
        {
            if (messages == null)
                return new List<MessageSummaryDTO>();

            return messages.Select(m => ToMessageSummaryDTO(m)).ToList();
        }
    }
}