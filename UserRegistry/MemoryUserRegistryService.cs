using System;
using System.Collections.Generic;

namespace UserRegistry
{
    public class MemoryUserRegistryService : IUserRegistryService
    {
        public Dictionary<string, List<string>> _users;

        public Action<string, string, List<string>> OnUserConnected { get; set; }
        public Action<string, string, List<string>> OnUserDisconnected { get; set; }

        public MemoryUserRegistryService()
        {
            _users = new Dictionary<string, List<string>>();
        }

        public void UserConnected(string userId, string connectionId)
        {
            if (!_users.ContainsKey(userId))
            {
                _users.Add(userId, new List<string>());
            }

            if (!_users[userId].Contains(connectionId))
            {
                _users[userId].Add(connectionId);
            }

            OnUserConnected?.Invoke(userId, connectionId, _users[userId]);
        }

        public void UserDisconnected(string userId, string connectionId)
        {
            if (!_users.ContainsKey(userId))
            {
                _users.Add(userId, new List<string>());
            }

            if (_users[userId].Contains(connectionId))
            {
                _users[userId].Remove(connectionId);
            }

            OnUserDisconnected?.Invoke(userId, connectionId, _users[userId]);
        }

        public List<string> GetUserConnections(string userId)
        {
            if (!_users.ContainsKey(userId))
            {
                _users.Add(userId, new List<string>());
            }

            return _users[userId];
        }

        public Dictionary<string, List<string>> GetAllUsers()
        {
            return _users;
        }
    }
}
