﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace COMP476Proj
{
    /// <summary>
    /// 2D bounding box with debugging tools.
    /// </summary>
    public class BoundingRectangle
    {
        #region Attributes

        private Vector2 center;
        private Vector2 dimensionsFromCenter;
        private Rectanglef boundingRectangle;

        public Rectanglef Bounds { get { return boundingRectangle; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Rectangle style constructor
        /// </summary>
        /// <param name="x">Left most X value</param>
        /// <param name="y">Top most Y value</param>
        /// <param name="width">Width of the box</param>
        /// <param name="height">Height of the box</param>
        public BoundingRectangle(float x, float y, float width, float height)
        {
            center = new Vector2(x + width / 2, y + height / 2);

            boundingRectangle = new Rectanglef(x, y, width, height);
            //boundingRectangle.IsEmpty;

            dimensionsFromCenter = new Vector2(width / 2, height / 2);
        }

        /// <summary>
        /// Bounding sphere style constructor
        /// </summary>
        /// <param name="center">Center of the box</param>
        /// <param name="radius">Radius from center</param>
        public BoundingRectangle(Vector2 center, float radius)
        {
            dimensionsFromCenter = new Vector2(radius, radius);

            this.center = center;

            boundingRectangle = new Rectanglef(center.X - radius, center.Y - radius, 2 * radius, 2 * radius);
        }

        /// <summary>
        /// Bounding sphere style constructor 2
        /// </summary>
        /// <param name="center">Center of the box</param>
        /// <param name="x">X radius from center</param>
        /// <param name="y">Y radius from center</param>
        public BoundingRectangle(Vector2 center, float x, float y)
        {
            dimensionsFromCenter = new Vector2(x, y);

            this.center = center;

            boundingRectangle = new Rectanglef(center.X - x, center.Y - y, 2 * x, 2 * y);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Update the bounding rectangle position
        /// </summary>
        /// <param name="center"></param>
        public void Update(Vector2 center)
        {
            this.center = center;
            boundingRectangle.X = center.X - dimensionsFromCenter.X;
            boundingRectangle.Y = center.Y - dimensionsFromCenter.Y;
        }

        /// <summary>
        /// Checks for collision with other bounding rectangle
        /// </summary>
        /// <param name="rectangle">Bounding rectangle to check collision with</param>
        /// <returns>True if there is a collision</returns>
        public bool Collides(BoundingRectangle rectangle)
        {
            return boundingRectangle.Intersects(rectangle.boundingRectangle);
        }

        /// <summary>
        /// Checks for collision with a rectangle
        /// </summary>
        /// <param name="rectangle">Rectangle to check collision with</param>
        /// <returns>True if there is a collision</returns>
        public bool Collides(Rectanglef rectangle)
        {
            return boundingRectangle.Intersects(rectangle);
        }

        /// <summary>
        /// Draw the bounding rectangle for debugging purposes
        /// </summary>
        /// <param name="graphicsDevice">Graphics device</param>
        /// <param name="spriteBatch">Sprite batch</param>
        public void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            Texture2D textureToDraw = new Texture2D(graphicsDevice, (int)dimensionsFromCenter.X * 2, (int)dimensionsFromCenter.Y * 2);

            Color[] data = new Color[textureToDraw.Width * textureToDraw.Height];

            for (int i = 0; i != data.Length; ++i)
            {
                data[i] = Color.Red;
            }

            textureToDraw.SetData(data);

            Vector2 drawPos = new Vector2(center.X - dimensionsFromCenter.X  - Camera.X, center.Y - dimensionsFromCenter.Y - Camera.Y);

            spriteBatch.Draw(textureToDraw, drawPos, Color.White);
        }

        #endregion
    }
}