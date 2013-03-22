#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace COMP476Proj
{
    public static class Camera
    {
        #region Fields
        private static Rectangle bounds = new Rectangle(0, 0, Game1.SCREEN_WIDTH, Game1.SCREEN_HEIGHT);
        public static int X { get { return bounds.X; } set { bounds.X = value; } }
        public static int Y { get { return bounds.Y; } set { bounds.Y = value; } }
        public static int Width { get { return bounds.Width; } }
        public static int Height { get { return bounds.Height; } }
        private static float scale;
        public static float Scale { get { return scale; } set { scale = Math.Abs(value); } }
        #endregion
    }
}
