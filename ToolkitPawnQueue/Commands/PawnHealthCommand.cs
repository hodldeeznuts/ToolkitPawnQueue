using System.Collections.Generic;
using System.Linq;
using RimWorld;
using ToolkitCore;
using ToolkitCore.Models;
using TwitchLib.Client.Models;
using Verse;

namespace ToolkitPawnQueue.Commands
{
    public class PawnHealthCommand : CommandMethod
    {
        private Pawn _target;

        public PawnHealthCommand(ToolkitChatCommand command) : base(command)
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

            MessageQueue.messageQueue.Enqueue($"@{chatCommand.ChatMessage.Username} → You're already in this colony.");
            return false;
        }

        public override void Execute(ChatCommand chatCommand)
        {
            if (_target == null)
            {
                return;
            }

            var response = $"@{chatCommand.ChatMessage.Username} → ";

            if (chatCommand.ArgumentsAsList.Any())
            {
                var query = chatCommand.ArgumentsAsList.FirstOrDefault();
                var result =
                    DefDatabase<PawnCapacityDef>.AllDefsListForReading.FirstOrDefault(
                        d => d.defName.EqualsIgnoreCase(query)
                    );

                response += result == null
                    ? $"No capacity named \"{query}\" could be found."
                    : GetCapacityReport(_target, result);
            }
            else
            {
                response += GetHealthReport(_target);
            }

            MessageQueue.messageQueue.Enqueue(response);
        }

        private static string GetHealthReport(Pawn subject)
        {
            var health = subject.health;
            var payload = health.summaryHealth.SummaryHealthPercent.ToStringPercent() + " ";

            payload += health.State != PawnHealthState.Mobile
                ? GetHealthStateFriendly(health.State)
                : GetMoodFriendly(subject);

            if (health.hediffSet.BleedRateTotal > 0.01f)
            {
                var ticks = HealthUtility.TicksUntilDeathDueToBloodLoss(subject);

                payload += " | ";
                payload += ticks >= 60000 ? "⌛" : "⏳ " + ticks.ToStringTicksToPeriod(shortForm: true);
                payload += " | ";
            }

            var source = DefDatabase<PawnCapacityDef>.AllDefsListForReading;

            if (subject.def.race.Humanlike)
            {
                source = source.Where(c => c.showOnHumanlikes).ToList();
            }
            else if (subject.def.race.Animal)
            {
                source = source.Where(c => c.showOnAnimals).ToList();
            }
            else if (subject.def.race.IsMechanoid)
            {
                source = source.Where(c => c.showOnMechanoids).ToList();
            }
            else
            {
                source.Clear();
                payload += "Unsupported race";
            }

            if (!source.Any())
            {
                return payload;
            }

            var container = "";

            foreach (var capacity in source.OrderBy(c => c.listOrder))
            {
                if (!PawnCapacityUtility.BodyCanEverDoCapacity(subject.RaceProps.body, capacity))
                {
                    continue;
                }

                container += capacity.GetLabelFor(subject).CapitalizeFirst() + ": ";
                container += HealthCardUtility.GetEfficiencyLabel(subject, capacity).First;
                container += ", ";
            }

            return payload + container.Substring(0, container.Length - 2);
        }

        private static string GetCapacityReport(Pawn subject, PawnCapacityDef capacity)
        {
            var payload = "";

            if (!PawnCapacityUtility.BodyCanEverDoCapacity(subject.RaceProps.body, capacity))
            {
                payload += Find.ActiveLanguageWorker.Pluralize(subject.kindDef.race.defName) + " ";
                payload += " are incapable of " + capacity.GetLabelFor(subject);

                return payload;
            }

            var impactors = new List<PawnCapacityUtility.CapacityImpactor>();
            payload += PawnCapacityUtility.CalculateCapacityLevel(subject.health.hediffSet, capacity, impactors)
                .ToStringPercent();
            payload += " | ";

            if (!impactors.Any())
            {
                return payload + "No health conditions";
            }

            var segments = new List<string>();

            foreach (var i in impactors)
            {
                if (i is PawnCapacityUtility.CapacityImpactorHediff)
                {
                    segments.Add(i.Readable(subject));
                }
            }

            foreach (var i in impactors)
            {
                if (i is PawnCapacityUtility.CapacityImpactorBodyPartHealth)
                {
                    segments.Add(i.Readable(subject));
                }
            }

            foreach (var i in impactors)
            {
                if (i is PawnCapacityUtility.CapacityImpactorCapacity)
                {
                    segments.Add(i.Readable(subject));
                }
            }

            foreach (var i in impactors)
            {
                if (i is PawnCapacityUtility.CapacityImpactorPain)
                {
                    segments.Add(i.Readable(subject));
                }
            }

            return payload + " | " + string.Join(", ", segments.ToArray());
        }

        private static string GetMoodFriendly(Pawn subject)
        {
            if (subject.MentalStateDef != null)
            {
                return "⚡";
            }

            var thresholdExtreme = subject.mindState.mentalBreaker.BreakThresholdExtreme;
            var moodLevel = subject.needs.mood.CurLevel;

            if (moodLevel < thresholdExtreme)
            {
                return "🤬";
            }

            if (moodLevel < thresholdExtreme + 0.0500000007450581)
            {
                return "😠";
            }

            if (moodLevel < subject.mindState.mentalBreaker.BreakThresholdMinor)
            {
                return "😣";
            }

            if (moodLevel < 0.649999976158142)
            {
                return "😐";
            }

            return moodLevel < 0.899999976158142 ? "🙂" : "😊";
        }

        private static string GetHealthStateFriendly(PawnHealthState state)
        {
            switch (state)
            {
                case PawnHealthState.Down:
                    return "💫";
                case PawnHealthState.Dead:
                    return "👻";
                default:
                    return string.Empty;
            }
        }
    }
}
