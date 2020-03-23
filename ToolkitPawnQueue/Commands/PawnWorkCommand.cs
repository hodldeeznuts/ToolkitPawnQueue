using System.Linq;
using ToolkitCore;
using ToolkitCore.Models;
using TwitchLib.Client.Models;
using Verse;

namespace ToolkitPawnQueue.Commands
{
    public class PawnWorkCommand : CommandMethod
    {
        private Pawn _target;

        public PawnWorkCommand(ToolkitChatCommand command) : base(command)
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

            MessageQueue.messageQueue.Enqueue($"@{chatCommand.ChatMessage.Username} → You're not in this colony.");
            return false;
        }

        public override void Execute(ChatCommand chatCommand)
        {
            if (_target == null)
            {
                return;
            }

            var response = $"@{chatCommand.ChatMessage.Username} → ";

            if (!_target.workSettings?.EverWork ?? true)
            {
                MessageQueue.messageQueue.Enqueue(response + "Your pawn is incapable of work.");
                return;
            }

            var naturalOrder = WorkTypeDefsUtility.WorkTypeDefsInPriorityOrder;
            var priorities = naturalOrder.OrderByDescending(p => _target.workSettings.GetPriority(p))
                .ThenBy(p => p.naturalPriority)
                .Reverse()
                .Where(p => _target.workSettings.GetPriority(p) > 0)
                .Select(p => p.ToString());

            response += $"{_target.Name.ToStringShort.CapitalizeFirst()}'s work priorities ";
            response += string.Join(", ", priorities.ToArray());

            MessageQueue.messageQueue.Enqueue(response);
        }
    }
}
