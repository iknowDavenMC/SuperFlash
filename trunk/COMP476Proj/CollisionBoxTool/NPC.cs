using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CollisionBoxTool
{
    public class NPC
    {
        public bool selected;
        public Vector2 position;
        public int X { get { return (int)position.X; } }
        public int Y { get { return (int)position.Y; } }
        private float radius = 16;
        private Texture2D tex;
        public enum Type { Civilian, DumbCop, SmartCop }
        public enum Mode { Wander, Static }
        public Type type;
        public Mode mode;
        public Color color;
        public NPC(float x, float y, Type type, Mode mode)
        {
            position = new Vector2(x, y);
            this.type = type;
            this.mode = mode;
            color = Color.Purple;
        }

        public bool pointInside(int x, int y)
        {
            float dx = x - position.X;
            float dy = y - position.Y;
            return (dx * dx + dy * dy) <= (radius * radius);
        }

        public void draw(SpriteBatch sb, Texture2D tex, Rectangle camera)
        {
            switch (type)
            {
                case Type.Civilian:
                    color = Color.LimeGreen;
                    break;
                case Type.DumbCop:
                    color = Color.Crimson;
                    break;
                case Type.SmartCop:
                    color = Color.Orange;
                    break;
            }
            Vector2 drawPos = new Vector2(position.X - radius - camera.X, position.Y - radius - camera.Y);
            sb.Draw(tex, drawPos, color);
        }
    }
}
