using MqService.Messages;

namespace MqService.Helper
{
    public static class QueueNameResolver
    {
        public static string GetUserQueueName(string userId)
        {
            return typeof(UserQueueMessage).FullName+"_UserQueue_"+userId;
        }
    }
}
