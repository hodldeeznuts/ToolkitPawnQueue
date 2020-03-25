using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ToolkitPawnQueue
{
    public static class PawnUtilities
    {
        // Utilities

        public static void NewLine(Rect rect, out Rect newRect)
        {
            newRect = new Rect(rect.x, rect.y + rect.height + 2f, rect.width, rect.height);
        }

        public static bool TryFindUserAssigned(Pawn pawn, out string username)
        {
            GameComponentPawnTracking component = Current.Game.GetComponent<GameComponentPawnTracking>();

            if (component.TryGetUserAssignedToPawn(pawn, out username))
            {
                return true;
            }

            username = null;
            return false;
        }

        // Selected User

        public static int usernamesInQueue => Current.Game.GetComponent<GameComponentPawnTracking>().viewersInQueue.Count;

        // GUI

        public static void DrawColonist(Rect rect, Pawn colonist)
        {
            Color color = new Color(1f, 1f, 1f, 1f);
            GUI.color = color;

            GUI.DrawTexture(rect, ColonistBar.BGTex);

            GUI.color = Color.white;

            GUI.DrawTexture(rect, PortraitsCache.Get(colonist, rect.size));
        }

        // Pawn Groups

        public static List<Pawn> currentListOfPawns = new List<Pawn>();

        public static PawnGroupType pawnGroupType = PawnGroupType.Colonists;

        public static string groupOfPawnsLabel = "None";

        public static List<Pawn> ListOfColonists(out string groupOfPawnsLabel, out Pawn selectedPawn)
        {
            var newList = new List<Pawn>();

            foreach (Map map in Find.Maps)
            {
                newList = newList.Concat(map.mapPawns.FreeColonists).ToList();
            }

            groupOfPawnsLabel = "Colonists";
            selectedPawn = newList.First();

            return newList;
        }

        public enum PawnGroupType
        {
            Colonists
        }
    }
}
