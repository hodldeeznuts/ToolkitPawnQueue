using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolkitCore;
using TwitchLib.Client.Models;
using Verse;

namespace ToolkitPawnQueue
{
    public class PawnCommands : TwitchInterfaceBase
    {
        public PawnCommands(Game game)
        {

        }

        public override void ParseCommand(ChatMessage msg)
        {
            GameComponentPawnTracking component = Current.Game.GetComponent<GameComponentPawnTracking>();

            if (PawnQueueSettings.joinableQueue && msg.Message.StartsWith("!joinqueue"))
            {
                if (component.TryAddUsernameToQueue(msg.Username))
                {
                    TwitchWrapper.SendChatMessage($"@{msg.Username} you have joined the pawn queue.");
                }
            }

            if (PawnQueueSettings.pawnSkillsCommand && msg.Message.StartsWith("!mypawnskills"))
            {

                if (!component.TryGetPawnAssignedToUser(msg.Username, out Pawn pawn))
                {
                    TwitchWrapper.SendChatMessage($"@{msg.Username} you are not in the colony.");
                    return;
                }

                string output = $"@{msg.Username} {pawn.Name.ToStringShort.CapitalizeFirst()}'s skill levels are ";

                List<SkillRecord> skills = pawn.skills.skills;

                for (int i = 0; i < skills.Count; i++)
                {
                    if (skills[i].TotallyDisabled)
                    {
                        output += $"{skills[i].def.LabelCap}: -";
                    }
                    else
                    {
                        output += $"{skills[i].def.LabelCap}: {skills[i].levelInt}";
                    }

                    if (skills[i].passion == Passion.Minor) output += "+";
                    if (skills[i].passion == Passion.Major) output += "++";

                    if (i != skills.Count - 1)
                    {
                        output += ", ";
                    }
                }

                TwitchWrapper.SendChatMessage(output);
            }

            if (PawnQueueSettings.pawnStoryCommand && msg.Message.StartsWith("!mypawnstory"))
            {
                if (!component.TryGetPawnAssignedToUser(msg.Username, out Pawn pawn))
                {
                    TwitchWrapper.SendChatMessage($"@{msg.Username} you are not in the colony.");
                    return;
                }

                string output = $"@{msg.Username} About {pawn.Name.ToStringShort.CapitalizeFirst()}: ";

                List<Backstory> backstories = pawn.story.AllBackstories.ToList();

                for (int i = 0; i < backstories.Count; i++)
                {
                    output += backstories[i].title;
                    if (i != backstories.Count - 1)
                    {
                        output += ", ";
                    }
                }

                output += " | " + pawn.gender;

                StringBuilder stringBuilder = new StringBuilder();
                WorkTags combinedDisabledWorkTags = pawn.story.DisabledWorkTagsBackstoryAndTraits;
                if (combinedDisabledWorkTags == WorkTags.None)
                {
                    stringBuilder.Append("(" + "NoneLower".Translate() + "), ");
                }
                else
                {
                    List<WorkTags> list = WorkTagsFrom(combinedDisabledWorkTags).ToList<WorkTags>();
                    bool flag2 = true;
                    foreach (WorkTags tags in list)
                    {
                        if (flag2)
                        {
                            stringBuilder.Append(tags.LabelTranslated().CapitalizeFirst());
                        }
                        else
                        {
                            stringBuilder.Append(tags.LabelTranslated());
                        }
                        stringBuilder.Append(", ");
                        flag2 = false;
                    }
                }
                string text = stringBuilder.ToString();
                text = text.Substring(0, text.Length - 2);

                output += " | Incapable of: " + text;

                output += " | Traits: ";

                List<Trait> traits = pawn.story.traits.allTraits;
                for (int i = 0; i < traits.Count; i++)
                {
                    output += traits[i].LabelCap;

                    if (i != traits.Count - 1)
                    {
                        output += ", ";
                    }
                }

                TwitchWrapper.SendChatMessage(output);
            }
        }

        private static IEnumerable<WorkTags> WorkTagsFrom(WorkTags tags)
        {
            foreach (WorkTags workTag in tags.GetAllSelectedItems<WorkTags>())
            {
                if (workTag != WorkTags.None)
                {
                    yield return workTag;
                }
            }
            yield break;
        }

    }
}
