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

        public event Action<Element>? OnChange;
        public event Action<Element, bool>? OnToggle;

        //Closed UI
        private readonly Button popUpButton;
        private readonly Label popUpButtonLabel;

        //Open UI
        private readonly StaticContainer popUpBackground;
        private readonly Button closeButton;

        private const int DayPerRow = 7;
        private static Vector2 DaySelectorPosition => new(50, 100);
        private const int DayButtonWidth = 52;
        private const int DayButtonOffset = 4;
        private readonly List<Button> daySelectors;
        private readonly List<Label> daySelectorLabels;
        
        private readonly Dropdown seasonDropdown;

        public DateTimePicker()
        {
            Clickable = false;

            popUpButton = new(Game1.mouseCursors, new(384, 396, 15, 15), new(ButtonWidth, ButtonHeight))
            {
                Callback = (e) =>
                { 
                    Open = !Open;
                    popUpBackground!.Enabled = Open;
                }
            };
            AddChild(popUpButton);

            popUpButtonLabel = new();
            AddChild(popUpButtonLabel);

            popUpBackground = new()
            {
                Size = new(Width, Height),
                OutlineColor = Color.Wheat,
                LocalPosition = EditorOffset
            };
            AddChild(popUpBackground);

            closeButton = new(Game1.mouseCursors, new Rectangle(337, 494, 12, 12), new(48, 48))
            {
                Callback = (e) =>
                {
                    Open = false;
                    popUpBackground!.Enabled = false;
                },
                LocalPosition = new Vector2(Width, -12) + EditorOffset
            };
            popUpBackground.AddChild(closeButton);

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
                    }
                });
                popUpBackground.AddChild(daySelectors[i]);

                //Label
                string labelString = (i + 1).ToString();
                
                daySelectorLabels.Add(new()
                {
                    String = labelString,
                    Font = Game1.smallFont,
                    NonBoldShadow = false,
                });
                Vector2 labelPos = new(
                    buttonPos.X + ((DayButtonWidth - daySelectorLabels[i].Width) / 2),
                    buttonPos.Y + ((DayButtonWidth - daySelectorLabels[i].Height) / 2));
                daySelectorLabels[i].LocalPosition = labelPos;
                popUpBackground.AddChild(daySelectorLabels[i]);
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

                popUpBackground.AddChild(name);
            }

            //Season dropdown
            seasonDropdown = new()
            {
                Choices = new[]
                {
                    StardewValley.Season.Spring.ToString(),
                    StardewValley.Season.Summer.ToString(),
                    StardewValley.Season.Fall.ToString(),
                    StardewValley.Season.Winter.ToString()
                },
                Labels = new[]
                {
                    StardewValley.Season.Spring.ToString(),
                    StardewValley.Season.Summer.ToString(),
                    StardewValley.Season.Fall.ToString(),
                    StardewValley.Season.Winter.ToString()
                },
                MaxValuesAtOnce = 4,
                LocalPosition = new(500, 250)
            };
            popUpBackground.AddChild(seasonDropdown);

            SetDay(Day);
        }

        public void SetDay(int day)
        {
            daySelectors[Day - 1].IdleTint = Button.IdleTintColour; //Reset the previous day button colour
            daySelectors[Day - 1].HoverTint = Button.HoverTintColour;
            daySelectorLabels[Day - 1].IdleTextColor = Game1.textColor; //Reset the previous day label colour

            Day = day;
            UpdateDateLabel();
            OnChange?.Invoke(this);

            daySelectors[Day - 1].IdleTint = Color.LightSeaGreen; //Highlight the current day button
            daySelectors[Day - 1].HoverTint = Color.SeaGreen;
            daySelectorLabels[Day - 1].IdleTextColor = Color.White; //Change label to white for better contrast
        }

        public override string ToString()
        {
            return $"{Date.ToLocaleString()} - {Time} {(Time < 1200 ? "am" : "pm")}";
        }

        private void UpdateDateLabel()
        {
            popUpButtonLabel.String = ToString();
            popUpButtonLabel.LocalPosition = new(
                (ButtonWidth - popUpButtonLabel.Width) / 2,
                (ButtonHeight - popUpButtonLabel.Height) / 2);
        }

        public override void Update(bool isOffScreen = false)
        {
            base.Update(isOffScreen);
        }

        public override void Draw(SpriteBatch b)
        {
            popUpButton.Draw(b);
            popUpButtonLabel.Draw(b);

            if (Open)
            {
                const int padding = 64;
                Rectangle contentArea = new(
                    (int)popUpBackground.Position.X - padding,
                    (int)popUpBackground.Position.Y - padding,
                    popUpBackground.Width + 2 * padding,
                    popUpBackground.Height + 2 * padding);

                Utilities.InScissorRectangle(b, contentArea, contentBatch =>
                {
                    popUpBackground.Draw(contentBatch);
                });
            }
        }
    }
}
