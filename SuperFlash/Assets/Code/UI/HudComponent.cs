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

    public class HudComponent
    {
        private Texture2D texture;
        private Vector2 position;
        private Vector2 origin;
        private Vector2 size;
        private float scale;
        private Rectangle rectangle;
        private float alpha;
        // TO-DO: Make methods to change the center position from center to origin 

        public HudComponent(Vector2 position, Vector2 size)
        {
            this.position = position;
            //Default origin is set to center of image
            this.origin = new Vector2(size.X / 2, size.Y / 2);
            this.scale = 1.0f;
            this.size = size;
            this.rectangle = new Rectangle(0, 0, (int)(size.X), (int)(size.Y));
            this.alpha = 1.0f;
        }

        public void LoadContent(Texture2D texture)
        {
            this.texture = texture;
        }
        public void Update(GameTime gameTime)
        {

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, float scale, Vector2 offset)
        {
            spriteBatch.Draw(texture, position*scale+offset, rectangle, Color.White*alpha, 0.0f, origin, scale, SpriteEffects.None, 0f);
        }

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
            //this.size.X = x;
            this.rectangle.Width = x;
        }
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
        public void setAlpha(float alpha)
        {
            this.alpha = alpha;
        }
    }
}
