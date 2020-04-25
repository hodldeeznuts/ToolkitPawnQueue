using System.Linq;

using ToolkitCore;
using ToolkitCore.Models;

using TwitchLib.Client.Interfaces;

using Verse;

namespace ToolkitPawnQueue.CommandMethods
{
    public class PawnWorkCommand : CommandMethod
    {
        private Pawn _target;

        public PawnWorkCommand(ToolkitChatCommand command) : base(command)
        {
        }

        public override bool CanExecute(ITwitchCommand chatCommand)
        {
            if(!base.CanExecute(chatCommand) || !PawnQueueSettings.pawnWorkCommand)
            {
                return false;
            }

            var component = Current.Game.GetComponent<GameComponentPawnTracking>();

            if(component.TryGetPawnAssignedToUser(chatCommand.ChatMessage.Username, out _target))
            {
                return true;
            }

            TwitchWrapper.SendChatMessage($"@{chatCommand.ChatMessage.Username} → You're not in this colony.");
            return false;
        }

        public override void Execute(ITwitchCommand chatCommand)
        {
            if(_target == null)
            {
                return;
            }

            var response = $"@{chatCommand.ChatMessage.Username} → ";

            if(!_target.workSettings?.EverWork ?? true)
            {
                TwitchWrapper.SendChatMessage($"{response}Your pawn is incapable of work.");
                return;
            }

            var naturalOrder = WorkTypeDefsUtility.WorkTypeDefsInPriorityOrder;
            var priorities = naturalOrder.OrderByDescending(p => _target.workSettings.GetPriority(p))
                .ThenBy(p => p.naturalPriority)
                .Reverse()
                .Where(p => _target.workSettings.GetPriority(p) > 0)
                .Select(p => $"{p}: {_target.workSettings.GetPriority(p).ToString()}");

            response += "Your work priorities are: ";
            response += string.Join(", ", priorities.ToArray());

            TwitchWrapper.SendChatMessage(response);
        }
    }
}
