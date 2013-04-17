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
        private static float maxX = Game1.SCREEN_WIDTH / 2;
        private static float maxY = Game1.SCREEN_HEIGHT / 2;
        private static float scale = 1f;
        private static float targetScale = 1f;
        private static float timeToZoom = 300;
        private static float timeZooming = 0;
        private static float oldScale = 1f;

        public static int X { get { return bounds.X; } set { bounds.X = value; } }
        public static int Y { get { return bounds.Y; } set { bounds.Y = value; } }
        public static int Width { get { return bounds.Width; } }
        public static int Height { get { return bounds.Height; } }
        public static float MaxX
        {
            get { return maxX; }
            set
            {
                value -= (Width / 2 / scale); // Keeps X centered without going past the intended X
                maxX = (value < Width / 2 / scale ? (Width / 2 / scale) : value);
            }
        }
        public static float MaxY
        {
            get { return maxY; }
            set
            {
                value -= (Height) / 2 / scale - HUD.getInstance().Height; // Keeps Y centered without going past the intended Y
                maxY = (value < (Height) / 2 / scale - HUD.getInstance().Height ? ((Height) / 2 / scale - HUD.getInstance().Height) : value);
            }
        }
        public static float Scale
        {
            get { return scale; }
            set
            {
                if (value != targetScale)
                {
                    targetScale = Math.Abs(value);
                    oldScale = scale;
                    timeZooming = 0;
                }
            }
        }

        public static Entity Target;
        public static Vector2 Offset = new Vector2(0, 0);


        #endregion

        #region Public Methods
        public static void Update(GameTime gameTime)
        {
            //maxY = Map.HEIGHT - Height/2;
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
                X = (int)maxX;
            if (Y > maxY)
                Y = (int)maxY;

            if (Math.Abs(scale - targetScale) > float.Epsilon && timeZooming < timeToZoom)
            {
                timeZooming += gameTime.ElapsedGameTime.Milliseconds;
                maxY = Map.HEIGHT - (Height / 2 - HUD.getInstance().Height) / scale;
                maxX = Map.WIDTH - (Width / 2) / scale;
                scale = (timeZooming / timeToZoom) * (targetScale - oldScale) + oldScale;
                //maxY = 948.5f;// Height* scale;
            }
            else
            {
                timeZooming = 0;
                scale = targetScale;
            }
        }
        #endregion
    }
}
