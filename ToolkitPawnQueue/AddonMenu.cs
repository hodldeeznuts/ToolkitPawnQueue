using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolkitCore.Interfaces;
using ToolkitCore.Windows;
using UnityEngine;
using Verse;

namespace ToolkitPawnQueue
{
    public class AddonMenu : IAddonMenu
    {
        List<FloatMenuOption> IAddonMenu.MenuOptions() => new List<FloatMenuOption>
        {
            new FloatMenuOption("Pawn Queue", delegate()
            {
                PawnAssignmentWindow window = new PawnAssignmentWindow();
                Find.WindowStack.TryRemove(window.GetType());
                Find.WindowStack.Add(window);

            }),
            new FloatMenuOption("Settings", delegate ()
            {
                Window_ModSettings window = new Window_ModSettings(LoadedModManager.GetMod<PawnQueue>());
                Find.WindowStack.TryRemove(window.GetType());
                Find.WindowStack.Add(window);
            }),
            new FloatMenuOption("Help", delegate()
            {
                Application.OpenURL("https://github.com/hodldeeznuts/ToolkitPawnQueue/wiki");
            })
        };
    }
}
