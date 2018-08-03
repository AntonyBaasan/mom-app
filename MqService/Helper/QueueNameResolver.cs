using MqService.Messages;

namespace MqService.Helper
{
    public static class QueueNameResolver
    {
        public static string GetUserQueue(string userId)
        {
            return "UserQueue_"+userId+"_"+typeof(UserQueueMessage).FullName;
        }
    }
}
