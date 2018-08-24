using MqService;
using MqService.Helper;
using MqService.Messages.Contents;

namespace ExecutionService.Commands
{
    public class OpenModuleCommand : AbstractCommand
    {
        public string ModuleName;

        public override object Execute(ExecutionContext context)
        {
            var msg = new OpenModuleMessage()
            {
                Metadata = context.metaData,
                Name = ModuleName
            };

            context.messageService.Send(Channels.TRIDENT_USER, ChannelType.Direct, msg);

            return "Done";
        }
    }
}
