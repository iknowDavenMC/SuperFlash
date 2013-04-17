using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StreakerLibrary;

namespace CollisionBoxTool
{
    class Consumable
    {
        public Rectangle rect;
        public Vector2 position;
        public enum Type { MASS, SPEED, SLIP, TURN }
        public Type type;
        private Texture2D tex;
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
                    tex = SpriteDatabase.GetAnimation("pwr_turn").Texture;
                    break;
                case Type.MASS:
                    c = Color.Orange;
                    tex = SpriteDatabase.GetAnimation("pwr_mass").Texture;
                    break;
                case Type.SLIP:
                    c = Color.Crimson;
                    tex = SpriteDatabase.GetAnimation("pwr_slick").Texture;
                    break;
                case Type.SPEED:
                    c = Color.Yellow;
                    tex = SpriteDatabase.GetAnimation("pwr_speed").Texture;
                    break;
                default:
                    c = Color.Purple;
                    tex = SpriteDatabase.GetAnimation("blank").Texture;
                    break;
            }
            spriteBatch.Draw(tex, draw, Color.White);
            //spriteBatch.Draw(blank, draw, c);
        }
    }
}
