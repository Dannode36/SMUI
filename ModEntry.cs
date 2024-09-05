using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using SMUI.Layout;
using SMUI.Elements;

namespace SMUI
{
    internal sealed class ModEntry : Mod
    {
        public static List<RootElement> updateList = new();

        public static void AddToUpdateList(RootElement root)
        {
            lock (updateList)
            {
                if (!updateList.Contains(root))
                {
                    updateList.Add(root);
                }
            }
        }
        public static bool RemoveFromUpdateList(RootElement root)
        {
            lock (updateList)
            {
                return updateList.Remove(root);
            }
        }

        public override void Entry(IModHelper helper)
        {
            LayoutHelper.helper = helper;
            LayoutHelper.monitor = Monitor;

            helper.Events.Display.Rendered += OnRendered;
            helper.Events.GameLoop.UpdateTicking += OnUpdateTicking;

            int numLayoutsFound = 0;
            Monitor.Log($"SMUI loaded succesfully. Found {numLayoutsFound} layout files", LogLevel.Info);

            //LayoutHelper.AddEventHandler("TestLog", (e) => { Monitor.Log("haha the button does something", LogLevel.Info); });
            //LayoutHelper.AddEventHandler("showText", (e) => { LayoutHelper.Layouts[0].uuidElements["heart"].Enabled = !LayoutHelper.Layouts[0].uuidElements["heart"].Enabled; });
            //LayoutHelper.LoadLayout("layout.xml");
        }

        private void OnUpdateTicking(object? sender, UpdateTickingEventArgs e)
        {
            foreach (var root in updateList)
            {
                root.Update();
            }
        }

        private void OnRendered(object? sender, RenderedEventArgs e)
        {
            LayoutHelper.Layouts[0].root.Draw(e.SpriteBatch);
            Game1.activeClickableMenu?.drawMouse(e.SpriteBatch);
        }
    }
}
