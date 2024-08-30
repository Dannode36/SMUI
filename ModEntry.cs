using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace SMUI
{
    internal sealed class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            LayoutHelper.helper = helper;
            LayoutHelper.monitor = Monitor;

            int numLayoutsFound = 0;
            Monitor.Log($"SMUI loaded succesfully. Found {numLayoutsFound} layout files", LogLevel.Info);
        }
    }
}
