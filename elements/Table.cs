using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;

namespace SMUI.Elements
{
    public class Table : Container
    {
        /*********
        ** Fields
        *********/
        private readonly List<Row> Rows = new();

        private Vector2 SizeImpl;

        private int RowHeightImpl;
        private readonly bool FixedRowHeight;
        private int ContentHeight;

        /*********
        ** Accessors
        *********/
        public Vector2 Size
        {
            get => SizeImpl;
            set
            {
                SizeImpl = new Vector2(value.X, RowHeight == 0 ? 0 : ((int)value.Y) / RowHeight * RowHeight);
                UpdateScrollbar();
            }
        }

        public int RowHeight
        {
            get => RowHeightImpl;
            set
            {
                RowHeightImpl = value + RowPadding;
                UpdateScrollbar();
            }
        }

        public int RowCount => Rows.Count;

        public Scrollbar Scrollbar { get; }

        /// <inheritdoc />
        public override int Width => (int)Size.X;

        /// <inheritdoc />
        public override int Height => (int)Size.Y;

        public int RowPadding { get; set; } = 16;

        public int RowSlip { get; set; } = 20;

        /*********
        ** Public methods
        *********/
        public Table(bool fixedRowHeight = true)
        {
            FixedRowHeight = fixedRowHeight;
            UpdateChildren = false; // table will update children itself
            Scrollbar = new Scrollbar
            {
                LocalPosition = new Vector2(0, 0)
            };
            AddChild(Scrollbar);
        }

        public void AddRow(Element[] elements)
        {
            Row row = new(elements);
            Rows.Add(row);
            AddChild(row);

            ContentHeight += FixedRowHeight ? RowHeight : row.MaxChildHeight + RowPadding;
            UpdateScrollbar();
        }

        /// <inheritdoc />
        public override void Update(bool isOffScreen = false)
        {
            base.Update(isOffScreen);
            if (IsHidden(isOffScreen))
                return;

            int rowTopPx = 0;
            foreach (var row in Rows)
            {
                row.LocalPosition = new(row.LocalPosition.X, rowTopPx - RowSlip - Scrollbar.TopRow * RowHeight);

                bool isChildOffScreen = isOffScreen || IsElementOffScreen(row);

                row.Update(isChildOffScreen);
                rowTopPx += FixedRowHeight ? RowHeight : row.MaxChildHeight + RowPadding;
            }

            if (rowTopPx != ContentHeight) {
                        ContentHeight = rowTopPx;
                Scrollbar.Rows = PxToRow(ContentHeight);
            }

            Scrollbar.Update();
        }

        public void ForceUpdateEvenHidden(bool isOffScreen = false)
        {
            int topPx = 0;
            foreach (var row in Rows)
            {
                int maxElementHeight = 0;
                foreach (var element in row.Elements)
                {
                    element.LocalPosition = new Vector2(element.LocalPosition.X, topPx - Scrollbar.ScrollPercent * Rows.Count * RowHeight);
                    bool isChildOffScreen = isOffScreen || IsElementOffScreen(element);

                    element.Update(isOffScreen: isChildOffScreen);
                    maxElementHeight = Math.Max(maxElementHeight, element.Height);
                }
                topPx += FixedRowHeight ? RowHeight : maxElementHeight + RowPadding;
            }
            ContentHeight = topPx;
            Scrollbar.Update(isOffScreen);
        }

        /// <inheritdoc />
        public override void Draw(SpriteBatch b)
        {
            if (IsHidden())
                return;

            // calculate draw area
            var backgroundArea = new Rectangle((int)Position.X - 32, (int)Position.Y - 32, (int)Size.X + 64, (int)Size.Y + 64);
            int contentPadding = 12;
            var contentArea = new Rectangle(backgroundArea.X + contentPadding, backgroundArea.Y + contentPadding, backgroundArea.Width - contentPadding * 2, backgroundArea.Height - contentPadding * 2);

            // draw background
            IClickableMenu.drawTextureBox(b, backgroundArea.X, backgroundArea.Y, backgroundArea.Width, backgroundArea.Height, Color.White);
            b.Draw(Game1.menuTexture, contentArea, new Rectangle(64, 128, 64, 64), Color.White); // Smoother gradient for the content area.

            // draw table contents
            // This uses a scissor rectangle to clip content taller than one row that might be
            // drawn past the bottom of the UI, like images or complex options.
            Element? renderLast = null;
            InScissorRectangle(b, contentArea, contentBatch =>
            {
                foreach (var row in Rows)
                {
                    if (IsElementOffScreen(row))
                    {
                        continue;
                    }
                    if (row == RenderLast)
                    {
                        renderLast = row;
                        continue;
                    }
                    row.Draw(contentBatch);
                }
            });
            renderLast?.Draw(b);

            Scrollbar.Draw(b);
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Get whether a child element is outside the table's current display area.</summary>
        /// <param name="element">The child element to check.</param>
        private bool IsElementOffScreen(Element element)
        {
            return
                element.Position.Y + element.Height < Position.Y
                || element.Position.Y > Position.Y + Size.Y;
        }

        private void UpdateScrollbar()
        {
            Scrollbar.LocalPosition = new Vector2(Size.X + 48, Scrollbar.LocalPosition.Y);
            Scrollbar.RequestHeight = (int)Size.Y;
            Scrollbar.Rows = PxToRow(ContentHeight);
            Scrollbar.FrameSize = (int)(Size.Y / RowHeight);
        }

        private static void InScissorRectangle(SpriteBatch spriteBatch, Rectangle area, Action<SpriteBatch> draw)
        {
            // render the current sprite batch to the screen
            spriteBatch.End();

            // start temporary sprite batch
            using SpriteBatch contentBatch = new(Game1.graphics.GraphicsDevice);
            GraphicsDevice device = Game1.graphics.GraphicsDevice;
            Rectangle prevScissorRectangle = device.ScissorRectangle;

            // render in scissor rectangle
            try
            {
                device.ScissorRectangle = area;
                contentBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, Utility.ScissorEnabled);

                draw(contentBatch);

                contentBatch.End();
            }
            finally
            {
                device.ScissorRectangle = prevScissorRectangle;
            }

            // resume previous sprite batch
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
        }

        private int PxToRow(int px)
        {
            return RowHeight == 0 ? 0 : (px + RowHeight - 1) / RowHeight;
        }
    }
}
