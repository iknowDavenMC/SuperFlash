using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace COMP476Proj
{
    public class Rectanglef
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;

        public float Left { get { return (Width >= 0 ? X : X + Width); } }
        public float Right { get { return (Width >= 0 ? X + Width : X); } }
        public float Top { get { return (Height >= 0 ? Y : Y + Height); } }
        public float Bottom { get { return (Height >= 0 ? X : X + Height); } }

        public bool isEmpty { get { return X == 0 && Y == 0 && Width == 0 && Height == 0; } }

        public static Rectanglef Empty { get { return new Rectanglef(); } }

        public Rectanglef()
        {
            X = 0; Y = 0; Width = 0; Height = 0;
        }

        public Rectanglef(float x, float y, float width, float height)
        {
            X = x; Y = y; Width = width; Height = height;
            X = Left; Y = Top; Width = Math.Abs(Width); Height = Math.Abs(Height);
        }

        public Rectanglef(Rectanglef rect)
        {
            X = rect.X;
            Y = rect.Y;
            Width = rect.Width;
            Height = rect.Height;
        }

        public bool Contains(float x, float y)
        {
            return x >= Left && x <= Right && y >= Top && y <= Bottom;
        }

        public bool Contains(Vector2 point)
        {
            return point.X >= Left && point.X <= Right && point.Y >= Top && point.Y <= Bottom;
        }

        public void Contains(ref Vector2 point, out bool result)
        {
            result = point.X >= Left && point.X <= Right && point.Y >= Top && point.Y <= Bottom;
        }

        public bool Contains(Rectanglef rectangle)
        {
            return rectangle.Left >= Left && rectangle.Right <= Right && rectangle.Top >= Top && rectangle.Bottom <= Bottom;
        }

        public void Contains(ref Rectanglef rectangle, out bool result)
        {
            result = rectangle.Left >= Left && rectangle.Right <= Right && rectangle.Top >= Top && rectangle.Bottom <= Bottom;
        }

        public override bool Equals(object obj)
        {
            Rectanglef rect = obj as Rectanglef;
            if (rect != null)
            {
                return X == rect.X && Y == rect.Y && Width == rect.Width && Height == rect.Height;
            }
            return base.Equals(obj);
        }

        public void Inflate(float horizontalAmount, float verticalAmount)
        {
            X = Left - horizontalAmount;
            Y = Top - horizontalAmount;
            Width = Math.Abs(Width) + horizontalAmount * 2;
            Height = Math.Abs(Height) + verticalAmount * 2;
        }

        public static Rectanglef Intersect(Rectanglef rect1, Rectanglef rect2)
        {
            Rectanglef result;
            if (rect1.Contains(rect2))
                result = new Rectanglef(rect2);
            else if (rect2.Contains(rect1))
                result = new Rectanglef(rect1);
            else if (!rect1.Intersects(rect2))
                result = new Rectanglef();
            else
            {
                result = new Rectanglef(
                    Math.Max(rect1.Left, rect2.Left),
                    Math.Min(rect1.Right, rect2.Right),
                    Math.Abs(rect1.Width - rect2.Width),
                    Math.Abs(rect1.Height - rect2.Height));
            }
            return result;
        }

        public static void Intersect(ref Rectanglef rect1, ref Rectanglef rect2, out Rectanglef result)
        {
            if (rect1.Contains(rect2))
                result = new Rectanglef(rect2);
            else if (rect2.Contains(rect1))
                result = new Rectanglef(rect1);
            else if (!rect1.Intersects(rect2))
                result = new Rectanglef();
            else
            {
                result = new Rectanglef(
                    Math.Max(rect1.Left, rect2.Left),
                    Math.Min(rect1.Right, rect2.Right),
                    Math.Abs(rect1.Width - rect2.Width),
                    Math.Abs(rect1.Height - rect2.Height));
            }
        }

        public bool Intersects(Rectanglef rect)
        {
            if (rect.Right < Left ||
                rect.Left > Right ||
                rect.Bottom < Top ||
                rect.Top > Bottom)
                return false;
            return true;
        }

        public static Rectanglef Union(Rectanglef rect1, Rectanglef rect2)
        {
            float newX = Math.Min(rect1.X, rect2.X);
            float newY = Math.Min(rect1.Y, rect2.Y);
            float newW = Math.Max(rect1.Right, rect2.Right) - newX;
            float newH = Math.Max(rect1.Bottom, rect2.Bottom) - newY;
            Rectanglef result = new Rectanglef(newX, newY, newW, newH);
            return result;
        }

        public static void Union(ref Rectanglef rect1, ref Rectanglef rect2, out Rectanglef result)
        {
            float newX = Math.Min(rect1.X, rect2.X);
            float newY = Math.Min(rect1.Y, rect2.Y);
            float newW = Math.Max(rect1.Right, rect2.Right) - newX;
            float newH = Math.Max(rect1.Bottom, rect2.Bottom) - newY;
            result = new Rectanglef(newX, newY, newW, newH);
        }
    }
}
