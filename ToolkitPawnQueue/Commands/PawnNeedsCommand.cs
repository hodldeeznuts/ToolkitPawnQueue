using System.Text;
using ToolkitCore;
using ToolkitCore.Models;
using TwitchLib.Client.Models;
using Verse;

namespace ToolkitPawnQueue.Commands
{
    public class PawnNeedsCommand : CommandMethod
    {
        private Pawn _target;
        
        public PawnNeedsCommand(ToolkitChatCommand command) : base(command)
        {
        }

        public override bool CanExecute(ChatCommand chatCommand)
        {
            if (!base.CanExecute(chatCommand))
            {
                return false;
            }

            var component = Current.Game.GetComponent<GameComponentPawnTracking>();

            if (component.TryGetPawnAssignedToUser(chatCommand.ChatMessage.Username, out _target))
            {
                return true;
            }

            TwitchWrapper.SendChatMessage($"@{chatCommand.ChatMessage.Username} → You're not in the colony.");
            return false;
        }

        public override void Execute(ChatCommand chatCommand)
        {
            if (_target == null)
            {
                return;
            }

            var response = $"@{chatCommand.ChatMessage.Username} → ";
            response += $"Your needs are: ";
            
            var container = new StringBuilder();
            var needs = _target.needs.AllNeeds;

            foreach (var need in needs)
            {
                container.Append($"{need.LabelCap}: {need.CurLevelPercentage.ToStringPercent()}");
                container.Append(", ");
            }

            var result = container.ToString();
            response += result.Substring(0, result.Length - 2);

            TwitchWrapper.SendChatMessage(response);
        }
    }
}
