using Microsoft.AspNetCore.SignalR;

namespace PRN222_Final_Project.Hubs
{
    public class OrderHub : Hub
    {
        public async Task NotifyNewOrder(string orderId, string customerName, decimal amount)
        {
            await Clients.All.SendAsync("ReceiveOrderNotification", orderId, customerName, amount);
        }
    }
}
