using System;
using System.Collections.Generic;

namespace UserRegistry
{
    public interface IUserRegistryService
    {
        Action<string, string, HashSet<string>> OnUserConnected { get; set; }
        Action<string, string, HashSet<string>> OnUserDisconnected { get; set; }

        int Count {get;}

        void Add(string userId, string connectionId);
        void Remove(string userId, string connectionId);
        HashSet<string> GetUserConnections(string userId);
        Dictionary<string, HashSet<string>> GetAllOnlineUsers();
    }
}
