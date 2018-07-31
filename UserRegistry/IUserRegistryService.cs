using System.Collections.Generic;

namespace UserRegistry
{
    public interface IUserRegistryService
    {
        void InsertConnection(string userId, string connectionId);
        void RemoveConnection(string userId, string connectionId);
        List<string> GetUserConnections(string userId);
    }
}
