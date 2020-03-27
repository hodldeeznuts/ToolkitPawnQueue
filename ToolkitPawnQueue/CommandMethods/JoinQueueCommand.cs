using System.Linq;
using ToolkitCore;
using ToolkitCore.Models;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using Verse;

namespace ToolkitPawnQueue.Commands
{
    public class JoinQueueCommand : CommandMethod
    {
        public JoinQueueCommand(ToolkitChatCommand command) : base(command)
        {
        }

        public override void Execute(ITwitchCommand twitchCommand)
        {
            GameComponentPawnTracking component = Current.Game.GetComponent<GameComponentPawnTracking>();

            if (component.TryAddUsernameToQueue(twitchCommand.Username))
            {
                TwitchWrapper.SendChatMessage($"@{twitchCommand.Username} → You've joined the pawn queue.");
            }
            else
            {
                TwitchWrapper.SendChatMessage($"@{twitchCommand.Username} unable to join pawn queue.");
            }
        }
    }
}
