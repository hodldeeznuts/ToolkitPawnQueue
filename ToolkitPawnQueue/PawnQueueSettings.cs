using UnityEngine;

using Verse;

namespace ToolkitPawnQueue
{
    public class PawnQueueSettings : ModSettings
    {
        // Features
        public static bool joinableQueue;

        public static bool renameAssignedPawns;

        // Commands
        public static bool pawnSkillsCommand;

        public static bool pawnStoryCommand;

        public static bool pawnWorkCommand;

        public static bool pawnBodyCommand;

        public static bool pawnGearCommand;

        public static bool pawnHealthCommand;

        public static bool pawnNeedsCommand;

        public void DoWindowContents(Rect rect)
        {
            Listing_Standard ls = new Listing_Standard();

            ls.Begin(rect);

            ls.Label("The name queue allows viewers to use command !joinqueue to enter. In the pawn assignment window, you can assign usernames manually (without the name queue), in order, or randomly from the list of usernames in the pawn queue");

            ls.CheckboxLabeled("Joinable Pawn Queue - !joinqueue", ref joinableQueue, "Let viewers join a pawn queue");

            ls.GapLine();

            if (ls.ButtonTextLabeled("Open/Manage Pawn Queue", "Pawn Queue"))
            {
                PawnAssignmentWindow window = new PawnAssignmentWindow();
                Find.WindowStack.TryRemove(typeof(PawnAssignmentWindow));
                Find.WindowStack.Add(window);
            }

            ls.GapLine();

            ls.CheckboxLabeled("Rename Pawns on Assignment", ref renameAssignedPawns, "Rename pawns that are assigned to have the viewers name");

            ls.GapLine();

            ls.Label("Enable/Disable pawn information commands");

            ls.CheckboxLabeled("My Pawn Skills - !mypawnskills", ref pawnSkillsCommand, "Let viewers get a message containing information about their pawns skills");

            ls.CheckboxLabeled("My Pawn Story - !mypawnstory", ref pawnStoryCommand, "Let Viewers get a message containing information about their pawns traits and backstory");

            ls.CheckboxLabeled("My Pawn Body - !mypawnbody", ref pawnBodyCommand, "Let viewers get a message containing information about their pawns body");

            ls.CheckboxLabeled("My Pawn Gear - !mypawngear", ref pawnGearCommand, "Let viewers get a message containing information about their pawns gear");

            ls.CheckboxLabeled("My Pawn Health - !mypawnhealth", ref pawnHealthCommand, "Let viewers get a message containing information about their pawns health");

            ls.CheckboxLabeled("My Pawn Work - !mypawnwork", ref pawnWorkCommand, "Let viewrs get a message containing information about their pawns work");

            ls.CheckboxLabeled("My Pawn Needs - !mypawnneeds", ref pawnNeedsCommand, "Let viewers get a message containing information about their pawns needs");

            ls.End();
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref joinableQueue, "joinableQueue", true);
            Scribe_Values.Look(ref renameAssignedPawns, "renameAssignedPawns", true);

            Scribe_Values.Look(ref pawnSkillsCommand, "pawnSkillsCommand", true);
            Scribe_Values.Look(ref pawnStoryCommand, "pawnStoryCommand", true);
            Scribe_Values.Look(ref pawnBodyCommand, "pawnBodyCommand", true);
            Scribe_Values.Look(ref pawnGearCommand, "pawnGearCommand", true);
            Scribe_Values.Look(ref pawnHealthCommand, "pawnHealthCommand", true);
            Scribe_Values.Look(ref pawnNeedsCommand, "pawnNeedsCommand", true);
            Scribe_Values.Look(ref pawnWorkCommand, "pawnWorkCommand", true);
        }
    }
}
