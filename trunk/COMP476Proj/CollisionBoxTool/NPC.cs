using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StreakerLibrary;

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
        public enum Type { Civilian, DumbCop, SmartCop, Streaker }
        public enum Mode { Wander, Static }
        public Type type;
        public Mode mode;
        public Color color;

        private const int PIXELS_HEAD_TO_TOE = 149;
        private const int PIXELS_LEFT_TO_CENTER = 88;

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
            Animation anim;
            switch (type)
            {
                case Type.Civilian:
                    anim = SpriteDatabase.GetAnimation("student1_static");
                    color = Color.LimeGreen;
                    break;
                case Type.DumbCop:
                    anim = SpriteDatabase.GetAnimation("cop_static");
                    color = Color.Crimson;
                    break;
                case Type.SmartCop:
                    anim = SpriteDatabase.GetAnimation("cop_static");
                    color = Color.Orange;
                    break;
                case Type.Streaker:
                    anim = SpriteDatabase.GetAnimation("streaker_static");
                    color = Color.Goldenrod;
                    break;
                default:
                    anim = SpriteDatabase.GetAnimation("student2_static");
                    color = Color.Orange;
                    break;
            }
            //color.A = 128;
            Vector2 offset = new Vector2(PIXELS_LEFT_TO_CENTER * 0.4f, PIXELS_HEAD_TO_TOE * 0.4f);

            Vector2 drawPos = new Vector2(position.X - camera.X, position.Y - camera.Y) - offset;
            Vector2 radOff = new Vector2(-radius, -radius);
            sb.Draw(tex, drawPos+radOff+offset, color);
            Rectangle sourceRect = new Rectangle(anim.FrameWidth, anim.YPos, anim.FrameWidth, anim.FrameHeight);
            sb.Draw(anim.Texture, drawPos, sourceRect, Color.White, 0, Vector2.Zero, 0.4f, SpriteEffects.None, 0);
        }
    }
}
