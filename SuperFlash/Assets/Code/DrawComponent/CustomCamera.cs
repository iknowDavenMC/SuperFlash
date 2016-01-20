using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using UnityEngine;

namespace COMP476Proj
{
    public static class CustomCamera : MonoBehaviour
    {
        private static Rect bounds = new Rect(SuperFlashGame.SCREEN_WIDTH / 2, SuperFlashGame.SCREEN_HEIGHT / 2, SuperFlashGame.SCREEN_WIDTH, SuperFlashGame.SCREEN_HEIGHT);
        private static float maxX = SuperFlashGame.SCREEN_WIDTH / 2;
        private static float maxY = SuperFlashGame.SCREEN_HEIGHT / 2;
        private static float scale = 1f;
        private static float targetScale = 1f;
        private static float timeToZoom = 300;
        private static float timeZooming = 0;
        private static float oldScale = 1f;

        public static float X { get { return bounds.x; } set { bounds.x = value; } }
				public static float Y { get { return bounds.y; } set { bounds.y = value; } }
				public static float Width { get { return bounds.width; } }
				public static float Height { get { return bounds.height; } }
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

        void Update()
        {
            //maxY = Map.height - Height/2;
            if (Target != null)
            {
                X = Target.X;
                Y = Target.Y;
                X += Offset.x;
                Y += Offset.y;
            }

            if (X < bounds.width / 2 / scale)
                X = (int)(bounds.width / 2 / scale);
            if (Y < bounds.height / 2 / scale)
                Y = (int)(bounds.height / 2 / scale);
            if (X > maxX)
                X = (int)maxX;
            if (Y > maxY)
                Y = (int)maxY;

            if (Math.Abs(scale - targetScale) > float.Epsilon && timeZooming < timeToZoom)
            {
                timeZooming += Time.deltaTime * 1000f;
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
    }
}
