using Microsoft.AspNetCore.SignalR;

namespace SupportApi.Hubs
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.Identity?.Name ?? connection.ConnectionId;
        }
    }
}
