using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ToolkitCore;
using ToolkitCore.Models;
using TwitchLib.Client.Models;
using Verse;

namespace ToolkitPawnQueue.Commands
{
    public class PawnStoryCommand : CommandMethod
    {
        private Pawn _target;

        public PawnStoryCommand(ToolkitChatCommand command) : base(command)
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

            TwitchWrapper.SendChatMessage($"@{chatCommand.ChatMessage.Username} → You're not in the colony.");
            return false;
        }

        public override void Execute(ChatCommand chatCommand)
        {
            if (_target == null)
            {
                return;
            }

            var response = $"@{chatCommand.ChatMessage.Username} → Backstory: ";
            response += string.Join(", ", _target.story.AllBackstories.Select(s => s.title).ToArray()).CapitalizeFirst();
            response += " | ";

            switch (_target.gender)
            {
                case Gender.Female:
                    response += "♀";
                    break;
                case Gender.Male:
                    response += "♂";
                    break;
                case Gender.None:
                    response += "⚪";
                    break;
            }

            var workContainer = new StringBuilder();
            var workTags = _target.story.DisabledWorkTagsBackstoryAndTraits;

            if (workTags == WorkTags.None)
            {
                workContainer.Append("(" + "NoneLower".Translate() + " ), ");
            }
            else
            {
                var tags = GetWorkTags(workTags);
                var start = true;

                foreach (var tag in tags)
                {
                    workContainer.Append(start ? tag.LabelTranslated().CapitalizeFirst() : tag.LabelTranslated());
                    workContainer.Append(", ");
                    start = false;
                }
            }

            var result = workContainer.ToString();
            result = result.Substring(0, result.Length - 2);

            response += " | Incapable of: " + result;
            response += " | Traits: ";
            response += string.Join(", ", _target.story.traits.allTraits.Select(t => t.LabelCap));

            TwitchWrapper.SendChatMessage(response);
        }

        private static IEnumerable<WorkTags> GetWorkTags(WorkTags tags)
        {
            return tags.GetAllSelectedItems<WorkTags>().Where(workTag => workTag != WorkTags.None);
        }
    }
}
