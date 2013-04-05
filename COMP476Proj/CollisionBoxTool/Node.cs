using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CollisionBoxTool
{
    class Node
    {
        private static int count;
        public int ID;
        public bool selected;
        public Vector2 position;
        private float radius = 16;
        public float Radius { get { return radius; } }
        private Texture2D tex;

        public Node(float x, float y)
        {
            ID = ++count;
            position = new Vector2(x, y);
        }

        public bool pointInside(int x, int y)
        {
            float dx = x - position.X;
            float dy = y - position.Y;
            return (dx * dx + dy * dy) <= (radius * radius);
        }

        public void draw(SpriteBatch sb, Texture2D tex, Color color, Rectangle camera)
        {
            Vector2 drawPos = new Vector2(position.X - radius - camera.X, position.Y - radius - camera.Y);
            sb.Draw(tex, drawPos, color);
        }
    }
}
