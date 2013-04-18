using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace COMP476Proj
{
    class Button
    {
        Texture2D texture;
        Vector2 position;
        Rectangle rectangle;

        bool isPressed = false;
        public Vector2 size;

        public bool isClicked;
        
        public Button(Texture2D newTexture, GraphicsDevice graphics)
        {
            texture = newTexture;
            size = new Vector2(211, 61);

        }
        public Button(Vector2 position, Vector2 size)
        {
            this.position = position;
            this.size = size;
        }
        
        public void Update(MouseState mouse)
        {
            rectangle = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
            Rectangle mouseRectangle = new Rectangle(mouse.X, mouse.Y, 1, 1);
            //This is where the hover of the mouse is
            if (mouseRectangle.Intersects(rectangle))
            {
                if (mouse.LeftButton == ButtonState.Pressed)
                    isPressed = true;
                if (mouse.LeftButton == ButtonState.Released && isPressed)
                {
                    isClicked = true;
                    isPressed = false;
                }

            }
            else
            {
                isClicked = false;
                isPressed = false;
            }
        }

        public bool buttonPressed()
        {
            return isClicked;
        }
        public Vector2 getSize()
        {
            return size;
        }
        public void setSize(Vector2 size)
        {
            this.size = size;
        }
        public void SetPosition(Vector2 newPosition)
        {
            position = newPosition;
        }

        public void Draw(SpriteBatch spritebatch)
        {
            //spritebatch.Draw(texture, rectangle, _color);
        }
    }
}
