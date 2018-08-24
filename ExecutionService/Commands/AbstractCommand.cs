using MqService.Messages;

namespace ExecutionService.Commands
{
    public abstract class AbstractCommand : ICommand
    {
        protected UserInfo _userInfo;

        public abstract object Execute(ExecutionContext context);

        public virtual void SetSender(UserInfo userInfo)
        {
            _userInfo = userInfo;
        }
    }
}
