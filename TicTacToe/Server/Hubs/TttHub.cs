using Microsoft.AspNetCore.SignalR;

namespace TicTacToe.Server.Hubs
{

    public class TttHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
