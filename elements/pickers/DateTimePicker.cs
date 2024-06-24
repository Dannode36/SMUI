using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValleyReminders.ui.pickers
{
    class DateTimePicker : Container
    {
        public override int Width => 500;

        public override int Height => 300;

        public SDate SDate { get; private set; } = SDate.Now();
        public int Time { get; private set; } = 600;

        public bool Open { get; private set; }

        //Closed UI
        private Button button;

        //Open UI
        private Button closeButton;
        StaticContainer background;

        public DateTimePicker()
        {
            button = new(Game1.mouseCursors, new(384, 396, 15, 15), new(100, 50))
            {
                Callback = (e) => { Open = true; },
            };
            AddChild(button);

            closeButton = new(Game1.mouseCursors, new Rectangle(337, 494, 12, 12), new(48, 48))
            {
                Callback = (e) => { Open = false; },
                LocalPosition = new(500, 0)
            };
            AddChild(closeButton);

            background = new()
            {
                Size = new(Width, Height),
                OutlineColor = Color.Wheat,
            };
            AddChild(background);
        }

        public override void Update(bool isOffScreen = false)
        {
            base.Update(isOffScreen);
            button.Update(isOffScreen);
            if (Open)
            {
                background.Update(isOffScreen);
                closeButton.Update(isOffScreen);
            }
        }

        public override void Draw(SpriteBatch b)
        {
            button.Draw(b);

            if (Open)
            {
                background.Draw(b);
                closeButton.Draw(b);
            }
        }
    }
}
