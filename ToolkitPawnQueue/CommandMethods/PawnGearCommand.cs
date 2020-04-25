using System.Linq;
using RimWorld;
using ToolkitCore;
using ToolkitCore.Models;
using TwitchLib.Client.Interfaces;
using UnityEngine;
using Verse;

namespace ToolkitPawnQueue.CommandMethods
{
    public class PawnGearCommand : CommandMethod
    {
        private Pawn _target;

        public PawnGearCommand(ToolkitChatCommand command) : base(command)
        {
        }

        public override bool CanExecute(ITwitchCommand chatCommand)
        {
            if (!base.CanExecute(chatCommand) || !PawnQueueSettings.pawnGearCommand)
            {
                return false;
            }

            var component = Current.Game.GetComponent<GameComponentPawnTracking>();

            if (component.TryGetPawnAssignedToUser(chatCommand.ChatMessage.Username, out _target))
            {
                return true;
            }

            TwitchWrapper.SendChatMessage($"@{chatCommand.ChatMessage.Username} → You're not in this colony.");
            return false;
        }

        public override void Execute(ITwitchCommand chatCommand)
        {
            if (_target == null)
            {
                return;
            }

            var payload = "";
            var sharp = CalculateArmorRating(_target, StatDefOf.ArmorRating_Sharp);
            var blunt = CalculateArmorRating(_target, StatDefOf.ArmorRating_Blunt);
            var heat = CalculateArmorRating(_target, StatDefOf.ArmorRating_Heat);

            if (sharp > 0)
            {
                payload += $"🗡️ {sharp.ToStringPercent()}";
            }

            if (blunt > 0)
            {
                payload += $"🧱 {blunt.ToStringPercent()}";
            }

            if (heat > 0)
            {
                payload += $"🔥 {heat.ToStringPercent()}";
            }

            var e = _target.equipment;

            if (e?.AllEquipmentListForReading?.Count > 0)
            {
                payload += "|";

                var equipment = e.AllEquipmentListForReading;
                payload += string.Join(", ", equipment.Select(item => item.LabelCap).ToArray());
            }

            TwitchWrapper.SendChatMessage(payload);
        }

        private static float CalculateArmorRating(Pawn subject, StatDef stat)
        {
            var rating = 0f;
            var value = Mathf.Clamp01(subject.GetStatValue(stat) / 2f);
            var parts = subject.RaceProps.body.AllParts;
            var apparel = subject.apparel?.WornApparel;

            foreach (var part in parts)
            {
                var cache = 1f - value;

                if (apparel?.Any() ?? false)
                {
                    foreach (var a in apparel)
                    {
                        if (!a.def.apparel.CoversBodyPart(part))
                        {
                            continue;
                        }

                        var v = Mathf.Clamp01(a.GetStatValue(stat) / 2f);
                        cache *= 1f - v;
                    }
                }

                rating += part.coverageAbs * (1f - cache);
            }

            return Mathf.Clamp(rating * 2f, 0f, 2f);
        }
    }
}
