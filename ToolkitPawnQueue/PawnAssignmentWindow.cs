using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolkitCore.Controllers;
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

            Rect pawnInformation = new Rect(0f, 140f, 240f, 24f);

            if (assignedUsername != null)
            {
                // Viewer Assigned
                Widgets.Label(pawnInformation, $"<color=green>Assigned User</color>: {assignedUsername}");

                PawnUtilities.NewLine(pawnInformation, out Rect unassignButton);
                unassignButton.width = 140f;

                if (Widgets.ButtonText(unassignButton, "Unassign Viewer"))
                {
                    component.UnassignUserFromPawn(assignedUsername);
                    UpdateInfoFromPawnChange();
                }
            }
            else
            {
                // Viewer Not Assigned
                Widgets.Label(pawnInformation, "<color=red>Not Assigned</color>");

                PawnUtilities.NewLine(pawnInformation, out Rect usernamesInfo);

                Widgets.Label(usernamesInfo, $"Users in Queue: {component.NumOfViewersInQueue}");

                PawnUtilities.NewLine(usernamesInfo, out Rect pickUsername);

                if (component.NumOfViewersInQueue > 0)
                {
                    Widgets.Label(pickUsername, "Pick Viewer: ");

                    pickUsername.x += 100f;
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
                    usernameFromQueue = Widgets.TextEntryLabeled(usernameInput, "Assign to Viewer: ", usernameFromQueue);
                }
                else
                {
                    usernameFromQueue = Widgets.TextEntryLabeled(usernameInput, "Assign to Viewer: ", "");
                }

                PawnUtilities.NewLine(usernameInput, out Rect assignButton);

                if (usernameFromQueue != null & Widgets.ButtonText(assignButton, "Confirm Assignment"))
                {
                    bool viewerDataExists = ViewerController.ViewerExists(usernameFromQueue);

                    string pawnName = usernameFromQueue;

                    if (viewerDataExists)
                    {
                        string displayName = ViewerController.GetViewer(usernameFromQueue).DisplayName;
                        if (displayName != null && displayName != string.Empty)
                        {
                            pawnName = displayName;
                        }
                    }

                    component.TryAssigningUserToPawn(pawnName, selectedPawn);
                    UpdateInfoFromPawnChange();
                }
            }

            Text.Anchor = TextAnchor.UpperLeft;
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
            component.TryGetRandomViewerFromQueue(out usernameFromQueue);
            usernameIterator = 0;
        }

        string usernameFromQueue = null;

        // PawnTracking

        GameComponentPawnTracking component = Current.Game.GetComponent<GameComponentPawnTracking>();

        // Selected Pawn

        Pawn selectedPawn = null;

        int iterator = 0;

        int usernameIterator = 0;

        string assignedUsername = null;

        void UpdateInfoFromPawnChange()
        {
            selectedPawn = PawnUtilities.currentListOfPawns[iterator];

            component.TryGetUserAssignedToPawn(selectedPawn, out assignedUsername);

            usernameFromQueue = null;
        }

        // GUI

        static float padding = 10f;
    }
}
