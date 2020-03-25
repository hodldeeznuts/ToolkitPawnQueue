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
            PawnUtilities.currentListOfPawns = PawnUtilities.ListOfColonists(out PawnUtilities.groupOfPawnsLabel, out selectedPawn);
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

            Widgets.Label(middleLabel, PawnUtilities.groupOfPawnsLabel);

            if (Widgets.ButtonText(rightArrow, ">", true, true, false))
            {

            }

            Text.Anchor = TextAnchor.MiddleLeft;

            // Right Side

            Rect pawnCard = new Rect(inRect.width - 124 - padding, 0, 124, 124);

            PawnUtilities.DrawColonist(pawnCard, selectedPawn);

            // Bottom Section

            Rect pawnInformation = new Rect(0f, 140f, inRect.width, 32f);

            if (assignedUsername != null)
            {
                // Viewer Assigned
                Widgets.Label(pawnInformation, $"<color=green>Assigned User</color>: {assignedUsername}");
            }
            else
            {
                // Viewer Not Assigned
                Widgets.Label(pawnInformation, "<color=red>Not Assigned</color>");

                PawnUtilities.NewLine(pawnInformation, out Rect usernamesInfo);

                Widgets.Label(usernamesInfo, $"Users in Queue: {PawnUtilities.usernamesInQueue}");

                PawnUtilities.NewLine(usernamesInfo, out Rect pickUsername);

                if (PawnUtilities.usernamesInQueue > 0)
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

                PawnUtilities.NewLine(pickUsername, out Rect usernameInput);

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

        void NextPawn()
        {
            if (iterator >= PawnUtilities.currentListOfPawns.Count - 1)
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
                iterator = PawnUtilities.currentListOfPawns.Count - 1;
            }
            else
            {
                iterator--;
            }

            UpdateInfoFromPawnChange();
        }

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

        string usernameFromQueue = null;

        // Selected Pawn

        Pawn selectedPawn = null;

        int iterator = 0;

        int usernameIterator = 0;

        string assignedUsername = null;

        void UpdateInfoFromPawnChange()
        {
            selectedPawn = PawnUtilities.currentListOfPawns[iterator];

            PawnUtilities.TryFindUserAssigned(selectedPawn, out assignedUsername);

            usernameFromQueue = null;
        }

        // GUI

        static float padding = 10f;
    }
}
