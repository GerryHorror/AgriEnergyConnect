using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AgriEnergyConnect.Services.Interfaces;
using System.Security.Claims;
using AgriEnergyConnect.Models;
using System.Linq;

namespace AgriEnergyConnect.Controllers
{
    // The MessageController class handles actions related to messaging in the system.
    // It provides methods for viewing inbox messages, sending messages, and viewing message details.
    // This controller is restricted to authenticated users.
    [Authorize]
    public class MessageController : Controller
    {
        // Services for managing messages and authentication.
        private readonly IMessageService _messageService;

        private readonly IAuthService _authService;

        // Constructor for initializing services.
        // Parameters:
        //   messageService - The service for managing messages.
        //   authService - The service for managing user authentication.
        public MessageController(IMessageService messageService, IAuthService authService)
        {
            _messageService = messageService;
            _authService = authService;
        }

        // Displays the inbox and sent messages for the current user.
        public async Task<IActionResult> Index()
        {
            // Get current user ID from claims.
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Retrieve inbox and sent messages.
            var inboxMessages = await _messageService.GetInboxMessagesAsync(userId);
            var sentMessages = await _messageService.GetSentMessagesAsync(userId);

            // Get all active users except the current user.
            var users = (await _authService.GetAllUsersAsync())
                .Where(u => u.UserId != userId && u.IsActive)
                .ToList();

            // Pass data to the view.
            ViewBag.InboxMessages = inboxMessages;
            ViewBag.SentMessages = sentMessages;
            ViewBag.Users = users;
            ViewBag.CurrentUserId = userId;
            return View();
        }

        // Displays the conversation between the current user and another user.
        public async Task<IActionResult> Conversation(int userId)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var messages = await _messageService.GetConversationAsync(currentUserId, userId);
            var otherUser = await _authService.GetUserByIdAsync(userId);

            ViewBag.OtherUser = otherUser;
            return View(messages);
        }

        // Sends a new message from the current user to a recipient.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(int RecipientId, string Subject, string Content)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Validate message input.
            if (!_messageService.ValidateMessageInput(Subject, Content, RecipientId))
            {
                TempData["ErrorMessage"] = "All fields are required.";
                return RedirectToAction("Index");
            }

            // Send the message.
            await _messageService.SendMessageAsync(currentUserId, RecipientId, Subject, Content);
            TempData["SuccessMessage"] = "Message sent successfully.";
            return RedirectToAction("Index");
        }

        // Marks a message as read for the current user.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(int messageId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _messageService.MarkAsReadAsync(messageId, userId);
            return result ? Ok() : BadRequest("Message not found or unauthorized");
        }

        // Displays the details of a specific message if the user has access.
        [HttpGet]
        public async Task<IActionResult> View(int id, bool reply = false)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Check if the user has access to the message.
            if (!await _messageService.HasMessageAccessAsync(id, userId))
            {
                return NotFound();
            }

            var messageDto = await _messageService.GetMessageDTOByIdAsync(id);
            ViewBag.Reply = reply;
            return View(messageDto);
        }

        // Returns the details of a specific message as JSON if the user has access.
        [HttpGet]
        public async Task<IActionResult> GetMessageDetails(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Check if the user has access to the message.
            if (!await _messageService.HasMessageAccessAsync(id, userId))
            {
                return NotFound();
            }

            var messageDto = await _messageService.GetMessageDTOByIdAsync(id);
            return Json(messageDto);
        }
    }
}