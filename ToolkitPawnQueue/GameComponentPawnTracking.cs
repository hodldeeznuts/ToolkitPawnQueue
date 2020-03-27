using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ToolkitPawnQueue
{
    public class GameComponentPawnTracking : GameComponent
    {
        public GameComponentPawnTracking(Game game)
        {

        }

        public override void GameComponentTick()
        {
            if (Find.TickManager.TicksGame % 1000 != 0) return;

            foreach (KeyValuePair<string, Pawn> pair in pawnsTracked)
            {
                if (pair.Value == null)
                {
                    pawnsTracked.Remove(pair.Key);
                }
            }
        }

        public int NumOfViewersInQueue => viewersInQueue.Count();

        #region PawnAssignment

        public bool TryGetUserAssignedToPawn(Pawn pawn, out string username)
        {
            foreach (KeyValuePair<string, Pawn> kv in pawnsTracked)
            {
                if (kv.Value == pawn)
                {
                    username = kv.Key;
                    return true;
                }
            }

            username = null;
            return false;
        }

        public bool TryGetPawnAssignedToUser(string username, out Pawn pawn)
        {
            return pawnsTracked.TryGetValue(username, out pawn);
        }

        #endregion

        #region PawnUsernameQueue

        public bool UsernameInQueue(string username)
        {
            return viewersInQueue.Contains(username);
        }

        public bool TryAddUsernameToQueue(string username)
        {
            if (!UsernameInQueue(username) && !TryGetPawnAssignedToUser(username, out Pawn pawn))
            {
                viewersInQueue.Add(username);
                return true;
            }

            return false;
        }

        public bool TryGetNextViewerFromQueue(out string username)
        {
            if (viewersInQueue.Count > 0)
            {
                username = viewersInQueue[0];
                return true;
            }

            username = null;
            return false;
        }

        public bool TryGetRandomViewerFromQueue(out string username)
        {
            if (NumOfViewersInQueue > 0)
            {
                username = viewersInQueue[new Random().Next(NumOfViewersInQueue - 1)];
                return true;
            }

            username = null;
            return false;
        }

        public bool TryAssigningUserToPawn(string username, Pawn pawnToAssign)
        {
            if (TryGetPawnAssignedToUser(username, out Pawn pawnAssigned)) return false;

            pawnsTracked.Add(username, pawnToAssign);

            if (viewersInQueue.Contains(username)) viewersInQueue.Remove(username);

            return true;
        }

        public void UnassignUserFromPawn(string username)
        {
            pawnsTracked.Remove(username);
            viewersInQueue.Add(username);
        }

        #endregion

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref pawnsTracked, "pawnsTracked", LookMode.Value, LookMode.Reference, ref viewerList, ref pawnList);
            Scribe_Collections.Look(ref viewersInQueue, "viewersInQueue", LookMode.Value);
        }

        public Dictionary<string, Pawn> pawnsTracked = new Dictionary<string, Pawn>();

        public List<string> viewersInQueue = new List<string>();

        public List<Pawn> pawnList = new List<Pawn>();
        public List<string> viewerList = new List<string>();
    }
}
