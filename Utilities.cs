using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace SMUI
{
    public class Utilities
    {
        public static string NameOfDay(int day, bool shortened = false)
        {
            return ((day - 1) % 7) switch
            {
                0 => shortened ? "Mon" : "Monday" ,
                1 => shortened ? "Tue" : "Tuesday",
                2 => shortened ? "Wed" : "Wednesday",
                3 => shortened ? "Thu" : "Thursday",
                4 => shortened ? "Fri" : "Friday",
                5 => shortened ? "Sat" : "Saturday",
                6 => shortened ? "Sun" : "Sunday",
                _ => "###"
            };
        }

        public static void InScissorRectangle(SpriteBatch spriteBatch, Rectangle area, Action<SpriteBatch> draw)
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

        public static Texture2D DoPaletteSwap(Texture2D baseTex, Texture2D from, Texture2D to)
        {
            var fromCols = new Color[from.Height];
            var toCols = new Color[to.Height];
            from.GetData(fromCols);
            to.GetData(toCols);
            return Utilities.DoPaletteSwap(baseTex, fromCols, toCols);
        }

        public static Texture2D DoPaletteSwap(Texture2D baseTex, Color[] fromCols, Color[] toCols)
        {
            var colMap = new Dictionary<Color, Color>();
            for (int i = 0; i < fromCols.Length; ++i)
            {
                colMap.Add(fromCols[i], toCols[i]);
            }

            var cols = new Color[baseTex.Width * baseTex.Height];
            baseTex.GetData(cols);
            for (int i = 0; i < cols.Length; ++i)
            {
                if (colMap.TryGetValue(cols[i], out Color color))
                    cols[i] = color;
            }

            var newTex = new Texture2D(Game1.graphics.GraphicsDevice, baseTex.Width, baseTex.Height);
            newTex.SetData(cols);
            return newTex;
        }

        public static T Clamp<T>(T min, T t, T max)
        {
            if (Comparer<T>.Default.Compare(min, t) > 0)
                return min;
            if (Comparer<T>.Default.Compare(max, t) < 0)
                return max;
            return t;
        }

        public static T Adjust<T>(T value, T interval)
        {
            if (value is float vFloat && interval is float iFloat)
                value = (T)(object)(float)((decimal)vFloat - ((decimal)vFloat % (decimal)iFloat));

            if (value is int vInt && interval is int iInt)
                value = (T)(object)(vInt - vInt % iInt);

            return value;
        }

        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            (rhs, lhs) = (lhs, rhs);
        }

        // https://stackoverflow.com/a/1626175/17827276
        public static Color ColorFromHsv(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value *= 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return new Color(v, t, p);
            else if (hi == 1)
                return new Color(q, v, p);
            else if (hi == 2)
                return new Color(p, v, t);
            else if (hi == 3)
                return new Color(p, q, v);
            else if (hi == 4)
                return new Color(t, p, v);
            else
                return new Color(v, p, q);
        }

        // https://stackoverflow.com/a/57385008
        public static IEnumerable<Color> GetColorGradient(Color from, Color to, int totalNumberOfColors)
        {
            if (totalNumberOfColors < 2)
            {
                throw new ArgumentException("Gradient cannot have less than two colors.", nameof(totalNumberOfColors));
            }

            double diffA = to.A - from.A;
            double diffR = to.R - from.R;
            double diffG = to.G - from.G;
            double diffB = to.B - from.B;

            int steps = totalNumberOfColors - 1;

            double stepA = diffA / steps;
            double stepR = diffR / steps;
            double stepG = diffG / steps;
            double stepB = diffB / steps;

            yield return from;

            for (int i = 1; i < steps; ++i)
            {
                yield return new Color(
                    c(from.R, stepR),
                    c(from.G, stepG),
                    c(from.B, stepB),
                    c(from.A, stepA));

                int c(int fromC, double stepC)
                {
                    return (int)Math.Round(fromC + stepC * i);
                }
            }

            yield return to;
        }
    }
}
