using System;
using System.Collections.Generic;

namespace UserRegistry
{
    public class MemoryUserRegistryService : IUserRegistryService
    {
        public Dictionary<string, List<string>> _users;

        public Action<string, List<string>> OnUserConnection;
        public Action<string, List<string>> OnUserDisconnection;

        public MemoryUserRegistryService()
        {
            _users = new Dictionary<string, List<string>>();
        }

        public void InsertConnection(string userId, string connectionId)
        {
            if (!_users.ContainsKey(userId))
            {
                _users.Add(userId, new List<string>());
            }

            if (!_users[userId].Contains(connectionId))
            {
                _users[userId].Add(connectionId);
            }

            OnUserConnection(userId, _users[userId]);
        }

        public void RemoveConnection(string userId, string connectionId)
        {
            throw new NotImplementedException();
        }

        public List<string> GetUserConnections(string userId)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, List<string>> GetAllUsers()
        {
            return _users;
        }
    }
}
