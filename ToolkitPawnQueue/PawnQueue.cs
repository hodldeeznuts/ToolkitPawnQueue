using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ToolkitPawnQueue
{
    public class PawnQueue : Mod
    {
        PawnQueueSettings settings;

        public PawnQueue(ModContentPack content) : base(content)
        {
            settings = GetSettings<PawnQueueSettings>();
        }

        public override string SettingsCategory() => "ToolkitPawnQueue";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            settings.DoWindowContents(inRect);
        }
    }
}
