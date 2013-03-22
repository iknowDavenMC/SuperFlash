using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// 2D bounding box with debugging tools.
/// </summary>
public class BoundingRectangle
{
    #region Attributes

    private Vector2 center;
    private Vector2 dimensionsFromCenter;
    private Rectangle boundingRectangle;
    private BasicEffect effect;

    #endregion

    #region Constructors

    /// <summary>
    /// Rectangle style constructor
    /// </summary>
    /// <param name="x">Left most X value</param>
    /// <param name="y">Top most Y value</param>
    /// <param name="width">Width of the box</param>
    /// <param name="height">Height of the box</param>
    public BoundingRectangle(int x, int y, int width, int height)
    {
        center = new Vector2(x + (float)width / 2, y + (float)height / 2);

        boundingRectangle = new Rectangle(x, y, width, height);

        dimensionsFromCenter = new Vector2((float)width / 2, (float)height / 2);
    }

    /// <summary>
    /// Bounding sphere style constructor
    /// </summary>
    /// <param name="center">Center of the box</param>
    /// <param name="radius">Radius from center</param>
    public BoundingRectangle(Vector2 center, int radius)
    {
        dimensionsFromCenter = new Vector2(radius, radius);

        this.center = center;

        boundingRectangle = new Rectangle((int)center.X - radius, (int)center.Y - radius, 2 * radius, 2 * radius);
    }

    /// <summary>
    /// Bounding rectangle style constructor
    /// </summary>
    /// <param name="center">Center of the box</param>
    /// <param name="x">Total width</param>
    /// <param name="y">Total height</param>
    public BoundingRectangle(Vector2 center, int x, int y)
    {
        dimensionsFromCenter = new Vector2(x / 2, y / 2);

        this.center = center;

        boundingRectangle = new Rectangle((int)center.X - x / 2, (int)center.Y - y / 2, x, y);
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
        boundingRectangle.X = (int)(center.X - dimensionsFromCenter.X);
        boundingRectangle.Y = (int)(center.Y - dimensionsFromCenter.Y);
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
    public bool Collides(Rectangle rectangle)
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
        Texture2D textureToDraw = new Texture2D(graphicsDevice, (int)dimensionsFromCenter.X, (int)dimensionsFromCenter.Y);

        Color[] data = new Color[textureToDraw.Width * textureToDraw.Height];

        for (int i = 0; i != data.Length; ++i)
        {
            data[i] = Color.Red;
        }

        textureToDraw.SetData(data);

        spriteBatch.Draw(textureToDraw, boundingRectangle, Color.White);
    }
     
    #endregion
}