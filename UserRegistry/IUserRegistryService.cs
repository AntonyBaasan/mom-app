using System;
using System.Collections.Generic;

namespace UserRegistry
{
    public interface IUserRegistryService
    {
        Action<string, string, List<string>> OnUserConnected { get; set; }
        Action<string, string, List<string>> OnUserDisconnected { get; set; }

        void UserConnected(string userId, string connectionId);
        void UserDisconnected(string userId, string connectionId);
        List<string> GetUserConnections(string userId);
        Dictionary<string, List<string>> GetAllOnlineUsers();
    }
}
