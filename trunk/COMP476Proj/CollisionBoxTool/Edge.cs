using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CollisionBoxTool
{
    class Edge
    {
        public Node start;
        public Node end;

        public Edge(Node s, Node e)
        {
            start = s;
            end = e;
        }

        public void draw(SpriteBatch sb, Texture2D tex, Color color, Rectangle camera)
        {
            if (start == null || end == null)
                return;
            float dx = end.position.X - start.position.X;
            float dy = end.position.Y - start.position.Y;
            double length = Math.Sqrt(dx * dx + dy * dy);
            Vector2 mid = new Vector2(start.position.X + dx / 2, start.position.Y + dy / 2);
            float angle = (float)Math.Atan2(dy, dx);
            Rectangle drawRect = new Rectangle(
                (int)start.position.X - camera.X,
                (int)start.position.Y - camera.Y,
                (int)length, 3);
            //sb.Draw(tex, drawRect, color);
            sb.Draw(tex, drawRect, null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
        }
    }
}
