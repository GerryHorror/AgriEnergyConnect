using AgriEnergyConnect.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AgriEnergyConnect.Controllers
{
    // The MessageController class handles actions related to messaging in the system.
    // It provides methods for viewing inbox messages, sending messages, and viewing message details.
    // This controller is restricted to authenticated users.
    [Authorize]
    public class MessageController : Controller
    {
        // Services for managing messages, farmers, and authentication.
        private readonly IMessageService _messageService;

        private readonly IFarmerService _farmerService;
        private readonly IAuthService _authService;

        // Constructor for the MessageController class.
        // Parameters:
        //   messageService - The service for managing messages.
        //   farmerService - The service for managing farmers.
        //   authService - The service for managing user authentication.
        public MessageController(IMessageService messageService, IFarmerService farmerService, IAuthService authService)
        {
            _messageService = messageService;
            _farmerService = farmerService;
            _authService = authService;
        }

        // Displays the inbox messages for the logged-in user.
        [HttpGet]
        public async Task<IActionResult> Inbox()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Retrieve inbox messages for the user
            var messages = await _messageService.GetInboxMessagesAsync(userId);

            return View(messages);
        }

        // Displays the form for sending a new message.
        // This action is restricted to users with the "Employee" role.
        [HttpGet]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Send()
        {
            // Retrieve all farmers to populate the recipient dropdown
            ViewBag.Farmers = await _farmerService.GetAllFamersAsync();

            return View();
        }

        // Handles the submission of the form for sending a new message.
        // Parameters:
        //   recipientId - The ID of the recipient of the message.
        //   subject - The subject of the message.
        //   content - The content of the message.
        // Returns a redirect to the sent messages page on success or redisplays the form on failure.
        [HttpPost]
        [Authorize(Roles = "Employee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(int recipientId, string subject, string content)
        {
            if (string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(content))
            {
                ModelState.AddModelError(string.Empty, "Subject and message content are required.");

                // Repopulate the farmers list for the form
                ViewBag.Farmers = await _farmerService.GetAllFamersAsync();

                return View();
            }

            var senderId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            try
            {
                // Send the message
                await _messageService.SendMessageAsync(senderId, recipientId, subject, content);
                return RedirectToAction(nameof(SentMessages));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error sending message: {ex.Message}");
                ViewBag.Farmers = await _farmerService.GetAllFamersAsync();
                return View();
            }
        }

        // Displays the sent messages for the logged-in user.
        // This action is restricted to users with the "Employee" role.
        [HttpGet]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> SentMessages()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Retrieve sent messages for the user
            var messages = await _messageService.GetSentMessagesAsync(userId);

            return View(messages);
        }

        // Displays the details of a specific message.
        // Parameters:
        //   id - The ID of the message to view.
        [HttpGet]
        public async Task<IActionResult> View(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Retrieve the message by ID
            var message = await _messageService.GetMessageByIdAsync(id);

            if (message == null)
            {
                return NotFound();
            }

            // Ensure the user is either the sender or recipient of the message
            if (message.SenderId != userId && message.RecipientId != userId)
            {
                return Forbid();
            }

            // Mark the message as read if the current user is the recipient
            if (message.RecipientId == userId && !message.IsRead)
            {
                await _messageService.MarkAsReadAsync(id, userId);
            }

            return View(message);
        }
    }
}