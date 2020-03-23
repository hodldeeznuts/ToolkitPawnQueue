using ToolkitCore;
using ToolkitCore.Models;
using TwitchLib.Client.Models;
using Verse;

namespace ToolkitPawnQueue.Commands
{
    public class JoinQueueCommand : CommandMethod
    {
        public JoinQueueCommand(ToolkitChatCommand command) : base(command)
        {
        }

        public override bool CanExecute(ChatCommand chatCommand)
        {
            if (!base.CanExecute(chatCommand))
            {
                return false;
            }

            var component = Current.Game.GetComponent<GameComponentPawnTracking>();

            return !component.UsernameInQueue(chatCommand.ChatMessage.Username);
        }

        public override void Execute(ChatCommand chatCommand)
        {
            var component = Current.Game.GetComponent<GameComponentPawnTracking>();

            if (component.TryAddUsernameToQueue(chatCommand.ChatMessage.Username))
            {
                MessageQueue.messageQueue.Enqueue(
                    $"@{chatCommand.ChatMessage.Username} → You've joined the pawn queue."
                );
            }
        }
    }
}
