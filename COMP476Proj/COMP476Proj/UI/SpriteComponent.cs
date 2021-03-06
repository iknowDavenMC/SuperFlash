using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace COMP476Proj
{

    public class SpriteComponent
    {
        /* -------------------------------------------------------------- */
        #region Attributes
        private Texture2D texture;
        private Vector2 position;
        private Vector2 origin;
        private Vector2 size;
        public float scale;
        private Vector2 vectorScale;
        private Rectangle rectangle;
        public float alpha;
        #endregion

        /* -------------------------------------------------------------- */
        #region Constructor and Load Content
        public SpriteComponent(Vector2 position, Vector2 size)
        {
            this.position = position;
            //Default origin is set to center of image
            this.origin = new Vector2(size.X / 2, size.Y / 2);
            this.scale = 1.0f;
            this.size = size;
            this.rectangle = new Rectangle(0, 0, (int)(size.X), (int)(size.Y));
            this.alpha = 1.0f;
            this.vectorScale = size;
        }
        
        public void LoadContent(Texture2D texture)
        {
            this.texture = texture;
        }
        #endregion
        
        /* -------------------------------------------------------------- */
        #region Update and Draw
        public void Update(GameTime gameTime)
        {

        }
        //Repositions from camera for HUD
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, float scale, Vector2 offset)
        {
            spriteBatch.Draw(texture, position*scale+offset, rectangle, Color.White*alpha, 0.0f, origin, scale, SpriteEffects.None, .1f);
        }
        //Standard draw method
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, rectangle, Color.White * alpha, 0.0f, origin, scale, SpriteEffects.None, .1f);
        }
        public void DrawWithScale(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, rectangle, Color.White * alpha, 0.0f, origin, vectorScale, SpriteEffects.None, 0f);
        }
        #endregion
        
        /* -------------------------------------------------------------- */
        #region Getters and Setters
        public Vector2 getPosition()
        {
            return position;
        }
        public void setSize(int x, int y)
        {
            this.size = new Vector2(x, y);
            this.rectangle = new Rectangle(0, 0, x, y);
        }
        public Vector2 getSize()
        {
            return size;
        }
        public void setXScale(int x)
        {
            this.rectangle.Width = x;
        }
        public void setAlpha(float alpha)
        {
            this.alpha = alpha;
        }
        public float getAlpha()
        {
            return alpha;
        }
        #endregion

        /* -------------------------------------------------------------- */
        #region Origin Adjusters
        public void setOriginLeft()
        {
            this.origin.X = 0.0f;
        }
        public void setOriginTopLeft()
        {
            this.origin.X = 0.0f;
            this.origin.Y = 0.0f;
        }
        public void setOriginBottomLeft()
        {
            this.origin.Y = size.Y;
            this.origin.X = 0.0f;
        }
        #endregion
    }
}
