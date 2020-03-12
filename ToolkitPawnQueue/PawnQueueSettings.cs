using UnityEngine;
using Verse;

namespace ToolkitPawnQueue
{
    public class PawnQueueSettings : ModSettings
    {
        // Features
        public static bool joinableQueue;


        // Commands
        public static bool pawnSkillsCommand;
        public static bool pawnStoryCommand;



        public void DoWindowContents(Rect rect)
        {
            Listing_Standard ls = new Listing_Standard();

            ls.Begin(rect);

            ls.Label("The name queue allows viewers to use command !joinqueue to enter. In the pawn assignment window, you can assign usernames manually (without the name queue), in order, or randomly from the list of available usernames.");

            ls.CheckboxLabeled("Joinable Name Queue - !joinqueue", ref joinableQueue, "Let viewers join a name queue");

            ls.GapLine();

            ls.Label("Enable/Disable pawn information commands");

            ls.CheckboxLabeled("My Pawn Skills - !mypawnskills", ref pawnSkillsCommand, "Let viewers get a message containing information about their pawns skills");

            ls.CheckboxLabeled("My Pawn Story - !mypawnstory", ref pawnStoryCommand, "Let Viewers get a message containing information about their pawns traits and backstory");

            ls.GapLine();

            if (ls.ButtonTextLabeled("Open/Manage name Queue", "Name Queue"))
            {
                PawnAssignmentWindow window = new PawnAssignmentWindow();
                Find.WindowStack.TryRemove(typeof(PawnAssignmentWindow));
                Find.WindowStack.Add(window);
            }

            ls.End();
        }

        public override void ExposeData()
        {
            Scribe_Values.Look<bool>(ref joinableQueue, "joinableQueue", true);
            Scribe_Values.Look<bool>(ref pawnSkillsCommand, "pawnSkillsCommand", true);
            Scribe_Values.Look<bool>(ref pawnStoryCommand, "pawnStoryCommand", true);
        }
    }
}