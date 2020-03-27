using System.Text;
using ToolkitCore;
using ToolkitCore.Models;
using TwitchLib.Client.Interfaces;
using Verse;

namespace ToolkitPawnQueue.CommandMethods
{
    public class PawnNeedsCommand : CommandMethod
    {
        private Pawn _target;

        public PawnNeedsCommand(ToolkitChatCommand command) : base(command)
        {
        }

        public override bool CanExecute(ITwitchCommand chatCommand)
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

        public override void Execute(ITwitchCommand chatCommand)
        {
            if (_target == null)
            {
                return;
            }

            var response = $"@{chatCommand.ChatMessage.Username} → Your needs are: ";

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
