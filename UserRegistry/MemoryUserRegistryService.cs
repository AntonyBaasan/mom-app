﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace UserRegistry
{
    public class MemoryUserRegistryService : IUserRegistryService
    {
        private readonly Dictionary<string, HashSet<string>> _users;

        public Action<string, string, HashSet<string>> OnConnected { get; set; }
        public Action<string, string, HashSet<string>> OnDisconnected { get; set; }

        public MemoryUserRegistryService()
        {
            _users = new Dictionary<string, HashSet<string>>();
        }

        public int Count { get { return _users.Count; } }

        public void Add(string userId, string connectionId)
        {
            lock (_users)
            {
                HashSet<string> connections;
                if (!_users.TryGetValue(userId, out connections))
                {
                    connections = new HashSet<string>();
                    _users.Add(userId, connections);
                }

                lock (connections)
                {
                    _users[userId].Add(connectionId);
                }

                OnConnected?.Invoke(userId, connectionId, _users[userId]);
            }
        }

        public void Remove(string userId, string connectionId)
        {
            lock (_users)
            {
                HashSet<string> connections;
                if (_users.TryGetValue(userId, out connections))
                {
                    lock (connections)
                    {
                        connections.Remove(connectionId);
                        if (connections.Count == 0)
                        {
                            _users.Remove(userId);
                        }
                    }
                }

                OnDisconnected?.Invoke(userId, connectionId, _users.ContainsKey(userId) ? _users[userId] :  new HashSet<string>());
            }
        }

        public HashSet<string> GetUserConnections(string userId)
        {
            if (!_users.ContainsKey(userId))
            {
                return new HashSet<string>();
            }

            return _users[userId];
        }

        public Dictionary<string, HashSet<string>> GetAllOnlineUsers()
        {
            return _users.Where(u => u.Value.Count > 0).ToDictionary(p => p.Key, p => p.Value);
        }
    }
}
