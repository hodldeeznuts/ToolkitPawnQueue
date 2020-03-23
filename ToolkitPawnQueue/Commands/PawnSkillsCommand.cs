using RimWorld;
using ToolkitCore;
using ToolkitCore.Models;
using TwitchLib.Client.Models;
using Verse;

namespace ToolkitPawnQueue.Commands
{
    public class PawnSkillsCommand : CommandMethod
    {
        private Pawn _target;

        public PawnSkillsCommand(ToolkitChatCommand command) : base(command)
        {
        }

        public override bool CanExecute(ChatCommand chatCommand)
        {
            if (!base.CanExecute(chatCommand))
            {
                return false;
            }

            var component = Current.Game.GetComponent<GameComponentPawnTracking>();

            if (component?.TryGetPawnAssignedToUser(chatCommand.ChatMessage.Username, out _target) == true)
            {
                return true;
            }

            MessageQueue.messageQueue.Enqueue($"@{chatCommand.ChatMessage.Username} → You're not in the colony.");
            return false;
        }

        public override void Execute(ChatCommand chatCommand)
        {
            if (_target == null)
            {
                return;
            }

            var response = $"@{chatCommand.ChatMessage.Username} → ";
            response += $"{_target.Name.ToStringShort.CapitalizeFirst()}'s skill levels are ";

            var skills = _target.skills.skills;

            for (var i = 0; i < skills.Count; i++)
            {
                var skill = skills[i];
                response += $"{skill.def.LabelCap}: ";
                response += skill.TotallyDisabled ? "-" : skill.levelInt.ToString();

                switch (skill.passion)
                {
                    case Passion.Major:
                        response += "🔥🔥";
                        break;
                    case Passion.Minor:
                        response += "🔥";
                        break;
                }

                if (i != skills.Count - 1)
                {
                    response += ", ";
                }
            }

            MessageQueue.messageQueue.Enqueue(response);
        }
    }
}
