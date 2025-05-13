using Microsoft.AspNetCore.Mvc;
using AgriEnergyConnect.Services.Interfaces;
using System.Security.Claims;

namespace AgriEnergyConnect.ViewComponents
{
    public class UnreadMessageCountViewComponent : ViewComponent
    {
        private readonly IMessageService _messageService;

        public UnreadMessageCountViewComponent(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = int.Parse(((ClaimsPrincipal)User).FindFirstValue(ClaimTypes.NameIdentifier));
            var unreadCount = await _messageService.GetUnreadMessageCountAsync(userId);
            return View(unreadCount);
        }
    }
}