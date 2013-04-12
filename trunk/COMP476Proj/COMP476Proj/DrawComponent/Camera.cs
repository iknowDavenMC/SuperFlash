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
        #region Members
        private static Rectangle bounds = new Rectangle(Game1.SCREEN_WIDTH / 2, Game1.SCREEN_HEIGHT / 2, Game1.SCREEN_WIDTH, Game1.SCREEN_HEIGHT);
        private static int maxX = Game1.SCREEN_WIDTH;
        private static int maxY = Game1.SCREEN_HEIGHT;
        private static float scale = 1f;

        public static int X { get { return bounds.X; } set { bounds.X = value; } }
        public static int Y { get { return bounds.Y; } set { bounds.Y = value; } }
        public static int Width { get { return bounds.Width; } }
        public static int Height { get { return bounds.Height; } }
        public static int MaxX
        {
            get { return maxX; }
            set
            {
                value -= (int)(Width / 2 / scale); // Keeps X centered without going past the intended X
                maxX = (value < Width / 2 / scale ? (int)(Width / 2 / scale) : value);
            }
        }
        public static int MaxY
        {
            get { return maxY; }
            set
            {
                value -= (int)((Height) / 2 / scale - HUD.getInstance().Height); // Keeps Y centered without going past the intended Y
                maxY = (value < (Height) / 2 / scale - HUD.getInstance().Height ? (int)((Height) / 2 / scale - HUD.getInstance().Height) : value);
            }
        }
        public static float Scale
        {
            get { return scale; }
            set
            {
                maxX += (int)((Width / 2) * scale);
                maxY += (int)((Height / 2) * scale);
                maxX -= (int)((Width / 2) / value);
                maxY -= (int)((Height / 2) / value);

                scale = Math.Abs(value);
            }
        }

        public static Entity Target;
        public static Vector2 Offset = new Vector2(0, 0);


        #endregion

        #region Public Methods
        public static void Update(GameTime gameTime)
        {
            if (Target != null)
            {
                X = (int)Target.X;
                Y = (int)Target.Y;
                X += (int)Offset.X;
                Y += (int)Offset.Y;
            }

            if (X < bounds.Width / 2 / scale)
                X = (int)(bounds.Width / 2 / scale);
            if (Y < bounds.Height / 2 / scale)
                Y = (int)(bounds.Height / 2 / scale);
            if (X > maxX)
                X = maxX;
            if (Y > maxY)
                Y = maxY;
        }
        #endregion
    }
}
