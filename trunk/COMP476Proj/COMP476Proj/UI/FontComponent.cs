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

    public class FontComponent
    {
        /* -------------------------------------------------------------- */
        #region Attributes
        private SpriteFont font;
        private String text;
        private Vector2 position;
        private Vector2 origin;
        public Vector2 size;
        public float scale;
        private Rectangle rectangle;
        public float alpha;
        public float timer;
        #endregion

        /* -------------------------------------------------------------- */
        #region Constructor and Load Content
        public FontComponent(Vector2 position, Vector2 size)
        {
            this.position = position;
            //Default origin is set to center of image
            this.origin = new Vector2(size.X / 2, size.Y / 2);
            this.scale = 1.0f;
            this.size = size;
            this.rectangle = new Rectangle(0, 0, (int)(size.X), (int)(size.Y));
            this.alpha = 1.0f;
            this.timer = 0.0f;
        }

        public void LoadContent(SpriteFont font)
        {
            this.font = font; 
        }
        public void setText(String text)
        {
            this.text = text;
        }
        #endregion

        /* -------------------------------------------------------------- */
        #region Update and Draw
        public void Update(GameTime gameTime)
        {

        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, float scale, Vector2 offset)
        {
            spriteBatch.DrawString(font, text, position * scale + offset, Color.White*alpha, 0f, Vector2.Zero, this.scale * scale, SpriteEffects.None, 0f);
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

        public void setFontScale(float scale)
        {
            this.scale = scale;
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
        public void setOriginCenter()
        {
            this.origin.X = this.size.X / 2;
            this.origin.Y = this.size.Y / 2;
        }
        #endregion
    }
}
