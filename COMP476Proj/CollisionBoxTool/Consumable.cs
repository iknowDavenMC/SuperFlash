using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CollisionBoxTool
{
    class Consumable
    {
        public Rectangle rect;
        public Vector2 position;
        public enum Type { MASS, SPEED, SLIP, TURN }
        public Type type;
        public Consumable(int x, int y, Type type)
        {
            rect = new Rectangle(x-16, y-16, 32, 32);
            position = new Vector2(x, y);
            this.type = type;
        }
        public void draw(SpriteBatch spriteBatch, Texture2D blank, Rectangle Camera)
        {
            Rectangle draw = new Rectangle(rect.X - Camera.X, rect.Y - Camera.Y, rect.Width, rect.Height);
            Color c = Color.Bisque;
            switch (type)
            {
                case Type.TURN:
                    c = Color.Coral;
                    break;
                case Type.MASS:
                    c = Color.Orange;
                    break;
                case Type.SLIP:
                    c = Color.Crimson;
                    break;
                case Type.SPEED:
                    c = Color.Yellow;
                    break;
                default:
                    c = Color.Purple;
                    break;
            }
            spriteBatch.Draw(blank, draw, c);
        }
    }
}
