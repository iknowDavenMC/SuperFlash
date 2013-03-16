using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// 2D bounding box with debugging tools.
/// </summary>
public class BoundingRectangle
{
    #region Static Attributes

        // Holds the indices required for drawing the bounding rectangle
        static private short[] indices = new short[] {0, 1, 1, 2, 2, 3, 3, 0};

    #endregion

    #region Attributes

        private Vector2 center;
        private Vector2 dimensionsFromCenter;
        private Rectangle boundingRectangle;
        private BasicEffect effect;

    #endregion

    #region Constructors

        // Rectangle style constructor
        public BoundingRectangle(int x, int y, int width, int height)
        {
            center = new Vector2(x + (float)width / 2, y + (float)height / 2);

            boundingRectangle = new Rectangle(x, y, width, height);

            dimensionsFromCenter = new Vector2((float)width / 2, (float)height / 2);
        }

        // Bounding sphere style constructor
        public BoundingRectangle(Vector2 center, float radius)
        {
            dimensionsFromCenter = new Vector2(radius, radius);

            this.center = center;

            boundingRectangle = new Rectangle(center.x - radius, center.y - radius, 2*radius, 2*radius);
        }

        // Bounding rectangle style constructor
        public BoundingRectangle(Vector2 center, float x, float y)
        {
            dimensionsFromCenter = new Vector2(x / 2, y / 2);

            this.center = center;

            boundingRectangle = new Rectangle(center.x - x / 2 , center.y - y / 2, x, y);
        }

    #endregion

    #region Methods

        // Update the bounding rectangle position
        public void Update(Vector2 center)
        {
            this.center = center;
            boundingRectangle.X = center.X - dimensionsFromCenter.X;
            boundingRectangle.Y = center.Y - dimensionsFromCenter.Y;
        }

        // Intersection Methods
        public bool Intersects(BoundingRectangle rectangle)
        {
            return boundingRectangle.Intersects(rectangle.boundingRectangle);
        }

        public bool Intersects(Rectangle rectangle)
        {
            return boundingRectangle.Intersects(rectangle);
        }

        // Draw the bounding rectangle for debugging purposes
        public void DrawBoundingCube(GraphicsDevice graphicsDevice, Matrix view, Matrix projection, Color color)
        {
            if (effect == null)
            {
                effect = new BasicEffect(graphicsDevice);
                effect.VertexColorEnabled = true;
                effect.TextureEnabled = false;
                effect.World = Matrix.Identity;
            }

            Vector2[] corners = new Vector2[4] {center + dimensionsFromCenter,
                                                new Vector2(center.X + dimensionsFromCenter.X, center.Y - dimensionsFromCenter.Y),
                                                center - dimensionsFromCenter,
                                                new Vector2(center.X - dimensionsFromCenter.X, center.Y + dimensionsFromCenter.Y)};

            VertexPositionColor[] primitiveList = new VertexPositionColor[corners.Length];

            for (int i = 0; i < corners.Length; i++)
            {
                primitiveList[i] = new VertexPositionColor(corners[i], color);
            }

            effect.View = view;
            effect.Projection = projection;

            // Draw the box with a LineList
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList, primitiveList, 0, 8, indices, 0, 12);
            }
        }

    #endregion
}