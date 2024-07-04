using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace SMUI.Elements.Pickers
{
    public class DateTimePicker : Container
    {
        public override int Width => 800;
        public override int Height => 500;

        public SDate Date => new(Day, Season, Year);
        public int Time { get; private set; } = 600;
        private int Day = Game1.dayOfMonth;
        private string Season = Game1.currentSeason;
        private int Year = Game1.year;

        public bool Open { get; private set; }

        private const int ButtonWidth = 600;
        private const int ButtonHeight = 100;
        private static Vector2 EditorOffset => new(50, 100);

        public Action<Element>? Callback { get; set; }

        //Closed UI
        private readonly Button button;
        private readonly Label label;
        private bool dateLabelDirty = false;

        //Open UI
        private readonly StaticContainer background;
        private readonly Button closeButton;

        private const int DayPerRow = 7;
        private static Vector2 DaySelectorPosition => new(50, 100);
        private const int DayButtonWidth = 52;
        private const int DayButtonOffset = 4;
        private readonly List<Button> daySelectors;
        private readonly List<Label> daySelectorLabels;

        public DateTimePicker()
        {
            Clickable = false;

            button = new(Game1.mouseCursors, new(384, 396, 15, 15), new(ButtonWidth, ButtonHeight))
            {
                Callback = (e) =>
                { 
                    Open = !Open;
                    background!.Enabled = Open;
                }
            };
            AddChild(button);

            label = new();
            AddChild(label);

            background = new()
            {
                Size = new(Width, Height),
                OutlineColor = Color.Wheat,
                LocalPosition = EditorOffset
            };
            AddChild(background);

            closeButton = new(Game1.mouseCursors, new Rectangle(337, 494, 12, 12), new(48, 48))
            {
                Callback = (e) =>
                {
                    Open = false;
                    background!.Enabled = false;
                },
                LocalPosition = new Vector2(Width, -12) + EditorOffset
            };
            background.AddChild(closeButton);

            //Init date buttons
            daySelectors = new();
            daySelectorLabels = new();
            for (int i = 0; i < 28; i++)
            {
                int day = i + 1;
                int row = i / DayPerRow;
                int col = i % DayPerRow;

                //Button
                Vector2 buttonPos = new(
                    DaySelectorPosition.X + ((DayButtonWidth + DayButtonOffset) * col), 
                    DaySelectorPosition.Y + ((DayButtonWidth + DayButtonOffset) * row));
                daySelectors.Add(new(Game1.mouseCursors, new(432, 439, 9, 9), new(DayButtonWidth, DayButtonWidth))
                {
                    LocalPosition = buttonPos,
                    Callback = (e) =>
                    {
                        SetDay(day);
                        Callback?.Invoke(this); //DateTimePicker callback
                    }
                });
                background.AddChild(daySelectors[i]);

                //Label
                string labelString = (i + 1).ToString();
                Vector2 labelPos = new(
                    buttonPos.X + ((DayButtonWidth - Label.MeasureString(labelString).X) / 2),
                    buttonPos.Y + ((DayButtonWidth - Label.MeasureString(labelString).Y) / 2));
                daySelectorLabels.Add(new()
                {
                    String = labelString,
                    Font = Game1.smallFont,
                    NonBoldShadow = false,
                    LocalPosition = labelPos
                });
                background.AddChild(daySelectorLabels[i]);
            }

            //Day names for the selector columns
            for (int i = 0; i < 7; i++)
            {
                Label name = new()
                {
                    Font = Game1.smallFont,
                    String = Utilities.NameOfDay(i + 1),
                };
                name.LocalPosition = new(
                        DaySelectorPosition.X + ((DayButtonWidth + DayButtonOffset) * i) + (DayButtonWidth - name.Width) / 2,
                        DaySelectorPosition.Y - 32);

                background.AddChild(name);
            }

            SetDay(Day);
        }

        public void SetDay(int day)
        {
            daySelectors[Day - 1].IdleTint = Button.IdleTintColour; //Reset the previous day button colour
            daySelectors[Day - 1].HoverTint = Button.HoverTintColour;
            daySelectorLabels[Day - 1].IdleTextColor = Game1.textColor; //Reset the previous day label colour

            Day = day;
            UpdateDateLabel();

            daySelectors[Day - 1].IdleTint = Color.LightSeaGreen; //Highlight the current day button
            daySelectors[Day - 1].HoverTint = Color.SeaGreen;
            daySelectorLabels[Day - 1].IdleTextColor = Color.White; //Change label to white for better contrast
        }

        private void UpdateDateLabel()
        {
            label.String = $"{Date.ToLocaleString()} \\@ {Time}";
            label.LocalPosition = new(
                (ButtonWidth - label.Width) / 2,
                (ButtonHeight - label.Height) / 2);
        }

        public override void Update(bool isOffScreen = false)
        {
            base.Update(isOffScreen);
        }

        public override void Draw(SpriteBatch b)
        {
            button.Draw(b);
            label.Draw(b);

            if (Open)
            {
                const int padding = 64;
                Rectangle contentArea = new(
                    (int)background.Position.X - padding,
                    (int)background.Position.Y - padding,
                    background.Width + 2 * padding,
                    background.Height + 2 * padding);

                Utilities.InScissorRectangle(b, contentArea, contentBatch =>
                {
                    background.Draw(contentBatch);
                });
            }
        }
    }
}
