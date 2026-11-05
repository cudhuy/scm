using Microsoft.AspNetCore.SignalR;

namespace Order.API.Helpers
{
    public class OrderHub : Hub
    {
        public async Task JoinOrderGroup(string orderId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"{orderId}");
        }

        public async Task LeaveOrderGroup(string orderId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{orderId}");
        }
    }
}
