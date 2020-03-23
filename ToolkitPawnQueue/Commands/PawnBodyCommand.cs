using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using ToolkitCore;
using ToolkitCore.Models;
using TwitchLib.Client.Models;
using Verse;

namespace ToolkitPawnQueue.Commands
{
    public class PawnBodyCommand : CommandMethod
    {
        private Pawn _target;

        public PawnBodyCommand(ToolkitChatCommand command) : base(command)
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

            if (_target.health.hediffSet?.hediffs?.Any() ?? true)
            {
                response += "No health conditions.";
                MessageQueue.messageQueue.Enqueue(response);
                return;
            }

            var grouped = GetVisibleHediffGroupsInOrder(_target);
            var tMin = _target.GetStatValue(StatDefOf.ComfyTemperatureMin).ToStringTemperature();
            var tMax = _target.GetStatValue(StatDefOf.ComfyTemperatureMax).ToStringTemperature();

            response += $"🌡️{tMin}~{tMax}";

            foreach (var group in grouped)
            {
                var part = group.Key?.LabelCap ?? "WholeBody".Translate();
                var container = new List<string>();

                foreach (var pGrouped in group.GroupBy(p => p.UIGroupKey))
                {
                    var segment = pGrouped.First().LabelCap;
                    var bleedCount = pGrouped.Count(i => i.Bleeding);
                    var total = pGrouped.Count();

                    if (total != 1)
                    {
                        segment += $" x{total.ToString()}";
                    }

                    if (bleedCount > 0)
                    {
                        segment = "🩸" + segment;
                    }

                    container.Add(segment);
                }

                response += part + ": " + string.Join(", ", container.ToArray());
            }

            MessageQueue.messageQueue.Enqueue(response);
        }

        private static float GetListPriority(BodyPartRecord record)
        {
            return record == null ? 9999999f : (float) record.height * 10000f;
        }

        private static IEnumerable<IGrouping<BodyPartRecord, Hediff>> GetVisibleHediffGroupsInOrder(Pawn pawn)
        {
            return GetVisibleHediffs(pawn)
                .GroupBy(x => x.Part)
                .OrderByDescending(x => GetListPriority(x.First().Part));
        }

        private static IEnumerable<Hediff> GetVisibleHediffs(Pawn pawn)
        {
            var missing = pawn.health.hediffSet.GetMissingPartsCommonAncestors();

            foreach (var part in missing)
            {
                yield return part;
            }

            var e = pawn.health.hediffSet.hediffs.Where(d => !(d is Hediff_MissingPart) && d.Visible);

            foreach (var item in e)
            {
                yield return item;
            }
        }
    }
}
