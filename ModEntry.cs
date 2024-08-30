using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace SMUI
{
    internal sealed class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            LayoutHelper.helper = helper;
            LayoutHelper.monitor = Monitor;

            LayoutHelper.AddEventHandler("TestLog", (e) => { Monitor.Log("haha the button does something", LogLevel.Info); });
            LayoutHelper.AddEventHandler("showText", (e) => { LayoutHelper.Layouts[0].uuidElements["heart"].Enabled = !LayoutHelper.Layouts[0].uuidElements["heart"].Enabled; });

            helper.Events.Display.Rendered += OnRendered;
            helper.Events.GameLoop.UpdateTicking += OnUpdateTicking;

            int numLayoutsFound = 0;
            Monitor.Log($"SMUI loaded succesfully. Found {numLayoutsFound} layout files", LogLevel.Info);

            LayoutHelper.LoadLayout("layout.xml");
        }

        private void OnUpdateTicking(object? sender, UpdateTickingEventArgs e)
        {
            LayoutHelper.Layouts[0].root.Update();
        }

        private void OnRendered(object? sender, RenderedEventArgs e)
        {
            LayoutHelper.Layouts[0].root.Draw(e.SpriteBatch);
            if(Game1.activeClickableMenu != null)
            {
                Game1.activeClickableMenu.drawMouse(e.SpriteBatch);
            }

        }
    }
}
