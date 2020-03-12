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
    public class PawnAssignmentWindow : Window
    {
        public PawnAssignmentWindow()
        {
            currentListOfPawns = ListOfColonists(out groupOfPawnsLabel, out selectedPawn);
            usernamesInQueue = Current.Game.GetComponent<GameComponentPawnTracking>().viewersInQueue.Count;

            this.doCloseButton = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            // Left Side

            Text.Anchor = TextAnchor.MiddleCenter;

            Rect leftArrow = new Rect(0f, 0f, 32f, 32f);
            Rect middleLabel = new Rect(leftArrow.width + padding, 0f, 200f, 32f);
            Rect rightArrow = new Rect(middleLabel.x + middleLabel.width + padding, 0f, 32f, 32f);

            if (Widgets.ButtonText(leftArrow, "<"))
            {
                PreviousPawn();
            }

            Widgets.Label(middleLabel, selectedPawn.NameFullColored);

            if (Widgets.ButtonText(rightArrow, ">"))
            {
                NextPawn();
            }

            leftArrow.y += 37f;
            middleLabel.y += 37f;
            rightArrow.y += 37f;

            if (Widgets.ButtonText(leftArrow, "<", true, true, false))
            {

            }

            Widgets.Label(middleLabel, pawnGroupType.ToString());

            if (Widgets.ButtonText(rightArrow, ">", true, true, false))
            {

            }

            Text.Anchor = TextAnchor.MiddleLeft;

            // Right Side

            Rect pawnCard = new Rect(inRect.width - 124 - padding, 0, 124, 124);

            DrawColonist(pawnCard, selectedPawn);

            // Bottom Section

            Rect pawnInformation = new Rect(0f, 140f, inRect.width, 32f);

            if (assignedUsername != null)
            {
                Widgets.Label(pawnInformation, $"<color=green>Assigned User</color>: {assignedUsername}");
            }
            else
            {
                Widgets.Label(pawnInformation, "<color=red>Not Assigned</color>");

                NewLine(pawnInformation, out Rect usernamesInfo);

                Widgets.Label(usernamesInfo, $"Users in Queue: {usernamesInQueue}");

                NewLine(usernamesInfo, out Rect pickUsername);

                if (usernamesInQueue > 0)
                {
                    Widgets.Label(pickUsername, "Pick User: ");

                    pickUsername.x += 80f;
                    pickUsername.width = 80f;
                    Text.Anchor = TextAnchor.MiddleCenter;

                    if (Widgets.ButtonText(pickUsername, "Next"))
                    {
                        NextUsername();
                    }

                    pickUsername.x += 100f;

                    if (Widgets.ButtonText(pickUsername, "Random"))
                    {
                        RandomUsername();
                    }

                    pickUsername.x = 0;
                    pickUsername.width = usernamesInfo.width;
                }

                NewLine(pickUsername, out Rect usernameInput);

                if (usernameFromQueue != null)
                {
                    usernameFromQueue = Widgets.TextEntryLabeled(usernameInput, "Assign to User: ", usernameFromQueue);
                }
                else
                {
                    usernameFromQueue = Widgets.TextEntryLabeled(usernameInput, "Assign to User: ", "");
                }

                Text.Anchor = TextAnchor.UpperLeft;
            }

        }

        // Pawn Groups

        List<Pawn> currentListOfPawns = new List<Pawn>();

        PawnGroupType pawnGroupType = PawnGroupType.Colonists;

        string groupOfPawnsLabel = "None";

        static List<Pawn> ListOfColonists(out string groupOfPawnsLabel, out Pawn selectedPawn)
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

        void NextPawn()
        {
            if (iterator >= currentListOfPawns.Count - 1)
            {
                iterator = 0;
            }
            else
            {
                iterator++;
            }

            UpdateInfoFromPawnChange();
        }

        void PreviousPawn()
        {
            if (iterator <= 0)
            {
                iterator = currentListOfPawns.Count - 1;
            }
            else
            {
                iterator--;
            }

            UpdateInfoFromPawnChange();
        }

        void UpdateInfoFromPawnChange()
        {
            selectedPawn = currentListOfPawns[iterator];

            TryFindUserAssigned(selectedPawn, out assignedUsername);

            usernameFromQueue = null;
        }

        // Selected Pawn

        Pawn selectedPawn = null;

        int iterator = 0;

        // Selected User

        string assignedUsername = null;

        string usernameFromQueue = null;

        int usernameIterator = 0;

        int usernamesInQueue = 0;

        void NextUsername()
        {
            GameComponentPawnTracking component = Current.Game.GetComponent<GameComponentPawnTracking>();

            if (usernameFromQueue == null)
            {
                usernameFromQueue = component.viewersInQueue[0];
                return;
            }

            if (usernameIterator < component.viewersInQueue.Count - 1)
            {
                usernameIterator++;
            }
            else
            {
                usernameIterator = 0;
            }

            usernameFromQueue = component.viewersInQueue[usernameIterator];
        }

        void RandomUsername()
        {
            Current.Game.GetComponent<GameComponentPawnTracking>().TryGetRandomViewerFromQueue(out usernameFromQueue);
            usernameIterator = 0;
        }

        // GUI

        static float padding = 10f;

        public static void DrawColonist(Rect rect, Pawn colonist)
        {
            Color color = new Color(1f, 1f, 1f, 1f);
            GUI.color = color;

            GUI.DrawTexture(rect, ColonistBar.BGTex);

            GUI.color = Color.white;

            GUI.DrawTexture(rect, PortraitsCache.Get(colonist, rect.size));
        }

        // Utilities

        static void NewLine(Rect rect, out Rect newRect)
        {
            newRect = new Rect(rect.x, rect.y + rect.height + 2f, rect.width, rect.height);
        }

        bool TryFindUserAssigned(Pawn pawn, out string username)
        {
            GameComponentPawnTracking component = Current.Game.GetComponent<GameComponentPawnTracking>();

            if (component.TryGetUserAssignedToPawn(pawn, out username))
            {
                return true;
            }

            username = null;
            return false;
        }

        enum PawnGroupType
        {
            Colonists
        }
    }
}
